using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using static DigiTransit10.ExtensionMethods.MapElementExtensions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class DigiTransitMap : UserControl
    {
        private readonly IGeolocationService _geolocationService;

        private static Task<IRandomAccessStream> _themeIconSource;
        private static Task<IRandomAccessStream> _themeIconPointerOverSource;
        private static Task<IRandomAccessStream> _themeIconSelectedSource;

        private static Task<IRandomAccessStream> _greyIconSource;

        private LiveGeolocationToken _liveUpdateToken;
        private CompassReading _lastKnownHeading;
        private Geopoint _lastKnownGeopoint;        

        public event EventHandler MapElementsChanged;
        public event TypedEventHandler<MapControl, object> MapCenterChanged;
        public event TypedEventHandler<MapControl, MapInputEventArgs> MapTapped;
        public event TypedEventHandler<MapControl, MapRightTappedEventArgs> MapRightTapped;
        public event TypedEventHandler<MapControl, MapElementClickEventArgs> MapElementClicked;

        public static readonly DependencyProperty MapServiceTokenProperty =
            DependencyProperty.Register(nameof(MapServiceToken), typeof(string), typeof(DigiTransitMap), new PropertyMetadata(null));
        public string MapServiceToken
        {
            get { return (string)GetValue(MapServiceTokenProperty); }
            set { SetValue(MapServiceTokenProperty, value); }
        }

        public static readonly DependencyProperty ShowUserOnMapProperty =
            DependencyProperty.Register(nameof(ShowUserOnMap), typeof(bool), typeof(DigiTransitMap), new PropertyMetadata(false,
                ShowUserOnMapChanged));
        private static void ShowUserOnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if (_this == null)
            {
                return;
            }

            if(e.NewValue.Equals(e.OldValue))
            {
                return;
            }

            bool newBool = (bool)e.NewValue;
            if (newBool)
            {
                _this.StartLiveUpdates();
            }
            else
            {
                _this.SelfMarker.Visibility = Visibility.Collapsed;
                _this.StopLiveUpdates();
            }
        }

        public bool ShowUserOnMap
        {
            get { return (bool)GetValue(ShowUserOnMapProperty); }
            set { SetValue(ShowUserOnMapProperty, value); }
        }

        public static readonly DependencyProperty ColoredMapLinesProperty =
            DependencyProperty.Register(nameof(ColoredMapLines), typeof(IList<ColoredMapLine>), typeof(DigiTransitMap), new PropertyMetadata(null,
                ColoredMapLinesChanged));
        private static void ColoredMapLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if (_this == null)
            {
                return;
            }

            var oldCollection = e.OldValue as INotifyCollectionChanged;
            var newCollection = e.NewValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= _this.OnColoredMapLinePointsCollectionChanged;
            }
            if (newCollection != null)
            {
                newCollection.CollectionChanged += _this.OnColoredMapLinePointsCollectionChanged;
            }

            IList<ColoredMapLine> newValue = e.NewValue as IList<ColoredMapLine>;
            if(newValue == null)
            {
                if (_this.DigiTransitMapControl.MapElements.OfType<MapPolyline>().Any())
                {
                    _this.SetMapLines(null);
                }
                return;
            }

            List<MapPolyline> newPolylines = new List<MapPolyline>();
            List<BasicGeoposition> currentLinePositions = new List<BasicGeoposition>();
            foreach (ColoredMapLine lineCollection in newValue)
            {
                for (int i = 0; i <= lineCollection.Count() - 2; i++)
                {
                    var startPoint = lineCollection[i];
                    var nextPoint = lineCollection[i + 1];
                    currentLinePositions.Add(startPoint.Coordinates);

                    Color nextColor = nextPoint.LineColor;
                    if (nextPoint == lineCollection.Last() || startPoint.LineColor != nextColor)
                    {
                        MapPolyline polyline = new MapPolyline();
                        polyline.Path = new Geopath(currentLinePositions);
                        polyline.StrokeColor = Color.FromArgb(192, startPoint.LineColor.R, startPoint.LineColor.G, startPoint.LineColor.B);
                        polyline.StrokeDashed = startPoint.IsLineDashed;
                        polyline.StrokeThickness = 6;
                        if(lineCollection.OptionalId != Guid.Empty)
                        {
                            MapElementExtensions.SetPoiId(polyline, lineCollection.OptionalId);
                        }
                        newPolylines.Add(polyline);

                        currentLinePositions = new List<BasicGeoposition>();
                    }
                }
            }

            _this.SetMapLines(newPolylines);
        }
        /// <summary>
        /// A collection of geopoints, between which lines are drawn. The line's color is determined by the starting point's <see cref="ColoredMapLinePoint.LineColor"/> property.
        /// </summary>
        public IList<ColoredMapLine> ColoredMapLines
        {
            get { return (IList<ColoredMapLine>)GetValue(ColoredMapLinesProperty); }
            set { SetValue(ColoredMapLinesProperty, value); }
        }
        private void OnColoredMapLinePointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }
            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                SetMapLines(new List<MapPolyline>());
            }

            if(e.NewItems != null)
            {
                List<MapPolyline> newPolylines = new List<MapPolyline>();
                List<BasicGeoposition> currentLinePositions = new List<BasicGeoposition>();
                foreach (ColoredMapLine lineCollection in e.NewItems.OfType<ColoredMapLine>())
                {
                    for (int i = 0; i <= lineCollection.Count() - 2; i++)
                    {
                        var startPoint = lineCollection[i];
                        var nextPoint = lineCollection[i + 1];
                        currentLinePositions.Add(startPoint.Coordinates);

                        Color nextColor = nextPoint.LineColor;
                        if (nextPoint == lineCollection.Last() || startPoint.LineColor != nextColor)
                        {
                            MapPolyline polyline = new MapPolyline();
                            polyline.Path = new Geopath(currentLinePositions);
                            polyline.StrokeColor = Color.FromArgb(192, startPoint.LineColor.R, startPoint.LineColor.G, startPoint.LineColor.B);
                            polyline.StrokeDashed = startPoint.IsLineDashed;
                            polyline.StrokeThickness = 6;
                            if (lineCollection.OptionalId != Guid.Empty)
                            {
                                MapElementExtensions.SetPoiId(polyline, lineCollection.OptionalId);
                            }
                            newPolylines.Add(polyline);

                            currentLinePositions = new List<BasicGeoposition>();
                        }
                    }
                }
                AddMapLines(newPolylines);
            }

            if(e.OldItems != null)
            {
                var removedLines = new List<MapPolyline>();
                foreach(ColoredMapLine oldLine in e.OldItems.OfType<ColoredMapLine>())
                {
                    MapPolyline removedLine = DigiTransitMapControl.MapElements
                        .OfType<MapPolyline>()
                        .FirstOrDefault(line => MapElementExtensions.GetPoiId(line) == oldLine.OptionalId);
                    if(removedLine != null)
                    {
                        removedLines.Add(removedLine);
                    }
                }
                RemoveMapLines(removedLines);
            }
        }

        public static readonly DependencyProperty PlacesProperty =
            DependencyProperty.Register("Places", typeof(IEnumerable<IMapPoi>), typeof(DigiTransitMap), new PropertyMetadata(null,
                new PropertyChangedCallback(OnPlacesChanged)));
        private static async void OnPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as DigiTransitMap;
            if(_this == null)
            {
                return;
            }

            //set up INotifyCollectionChangeds
            var oldCollection = e.OldValue as INotifyCollectionChanged;
            var newCollection = e.NewValue as INotifyCollectionChanged;
            if(oldCollection != null)
            {
                oldCollection.CollectionChanged -= _this.OnPlacesCollectionChanged;
            }
            if(newCollection != null)
            {
                newCollection.CollectionChanged += _this.OnPlacesCollectionChanged;
            }

            var newList = e.NewValue as IEnumerable<IMapPoi>;
            if(newList == null)
            {
                return;
            }

            List<MapIcon> icons = new List<MapIcon>();
            foreach(var place in newList.OfType<IMapPoi>())
            {
                MapIcon element = new MapIcon();
                element.Image = RandomAccessStreamReference.CreateFromStream(await _themeIconSource);
                element.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                element.Location = new Geopoint(place.Coords);
                element.Title = place.Name;
                element.NormalizedAnchorPoint = new Point(0.5, 1.0);
                MapElementExtensions.SetPoiId(element, place.Id);                
                icons.Add(element);
            }

            _this.SetMapIcons(icons);
        }
        public IEnumerable<IMapPoi> Places
        {
            get { return (IEnumerable<IMapPoi>)GetValue(PlacesProperty); }
            set { SetValue(PlacesProperty, value); }
        }

        private async void OnPlacesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(args.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }
            if(args.Action == NotifyCollectionChangedAction.Reset)
            {
                SetMapIcons(null);
            }

            if (args.NewItems != null)
            {
                var newIcons = new List<MapIcon>();
                foreach (IMapPoi place in args.NewItems.OfType<IMapPoi>())
                {
                    var element = new MapIcon();
                    element.Image = RandomAccessStreamReference.CreateFromStream(await _themeIconSource);
                    element.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                    element.Location = new Geopoint(place.Coords);
                    element.Title = place.Name;
                    element.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    element.SetValue(PoiIdProperty, place.Id);
                    MapElementExtensions.SetPoiId(element, place.Id);
                    newIcons.Add(element);
                }
                AddMapIcons(newIcons);
            }

            if (args.OldItems != null)
            {
                var removedIcons = new List<MapIcon>();
                foreach (IMapPoi place in args.OldItems.OfType<IMapPoi>())
                {
                    var removedIcon = DigiTransitMapControl.MapElements
                        .OfType<MapIcon>()
                        .FirstOrDefault(x => x.Location.Position.Equals(place.Coords));
                    removedIcons.Add(removedIcon);
                }
                RemoveMapIcons(removedIcons);
            }
        }

        // Using a DependencyProperty as the backing store for ColoredCircles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColoredCirclesProperty =
            DependencyProperty.Register("ColoredCircles", typeof(IEnumerable<ColoredGeocircle>), typeof(DigiTransitMap), new PropertyMetadata(null, OnColoredCirclesChanged));
        private static void OnColoredCirclesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if (_this == null)
            {
                return;
            }

            var oldCollection = e.OldValue as INotifyCollectionChanged;
            var newCollection = e.NewValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= _this.OnColoredCirclesCollectionChanged;
            }
            if (newCollection != null)
            {
                newCollection.CollectionChanged += _this.OnColoredCirclesCollectionChanged;
            }

            var newList = e.NewValue as IEnumerable<ColoredGeocircle>;
            if (newList == null || !newList.Any())
            {
                _this.SetMapPolygons(null);
                return;
            }

            List<MapPolygon> polygons = new List<MapPolygon>();
            foreach(var circle in newList)
            {
                var polygon = new MapPolygon();
                polygon.FillColor = circle.FillColor;
                polygon.Path = new Geopath(circle.CirclePoints.Select(x => x.Position));
                polygon.StrokeColor = circle.StrokeColor;
                polygon.StrokeThickness = circle.StrokeThickness;
                polygons.Add(polygon);
            }
            _this.SetMapPolygons(polygons);
        }

        public IEnumerable<ColoredGeocircle> ColoredCircles
        {
            get { return (IEnumerable<ColoredGeocircle>)GetValue(ColoredCirclesProperty); }
            set { SetValue(ColoredCirclesProperty, value); }
        }
        private void OnColoredCirclesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(args.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }
            if(args.Action == NotifyCollectionChangedAction.Reset)
            {
                SetMapPolygons(null); //replace with something smarter if we ever use the polygons collection for anything except circles
            }
            if(args.NewItems != null)
            {
                var newPolygons = new List<MapPolygon>();
                foreach(var circle in args.NewItems.OfType<ColoredGeocircle>())
                {
                    var polygon = new MapPolygon();
                    polygon.FillColor = circle.FillColor;
                    polygon.Path = new Geopath(circle.CirclePoints.Select(x => x.Position));
                    polygon.StrokeColor = circle.StrokeColor;
                    polygon.StrokeThickness = circle.StrokeThickness;
                    newPolygons.Add(polygon);
                }
                AddMapPolygons(newPolygons);
            }

            if(args.OldItems != null)
            {
                var removedPolygons = new List<MapPolygon>();
                foreach(var circle in args.OldItems.OfType<ColoredGeocircle>())
                {
                    var removedPolygon = DigiTransitMapControl.MapElements
                        .OfType<MapPolygon>()
                        .FirstOrDefault(x => x.Path.Positions.SequenceEqual(circle.CirclePoints.Select(y => y.Position)));
                    removedPolygons.Add(removedPolygon);
                }
                RemoveMapPolygons(removedPolygons);
            }
        }

        public static readonly DependencyProperty IsInteractionEnabledProperty =
            DependencyProperty.Register(nameof(IsInteractionEnabled), typeof(bool), typeof(DigiTransitMap), new PropertyMetadata(true,
                OnIsInteractionEnabledChanged));
        private static void OnIsInteractionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if(_this == null)
            {
                return;
            }

            bool oldEnabled = (bool)e.OldValue;
            bool newEnabled = (bool)e.NewValue;
            if(oldEnabled == newEnabled)
            {
                return;
            }

            if(newEnabled)
            {
                _this.DigiTransitMapControl.PanInteractionMode = MapPanInteractionMode.Auto;
                _this.DigiTransitMapControl.ZoomInteractionMode = MapInteractionMode.Auto;
                _this.DigiTransitMapControl.RotateInteractionMode = MapInteractionMode.Auto;
                _this.DigiTransitMapControl.TiltInteractionMode = MapInteractionMode.Auto;
                if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.Maps.MapControl", "AllowFocusOnInteraction"))
                {
                    _this.DigiTransitMapControl.AllowFocusOnInteraction = true;
                }
            }
            else
            {
                _this.DigiTransitMapControl.PanInteractionMode = MapPanInteractionMode.Disabled;
                _this.DigiTransitMapControl.ZoomInteractionMode = MapInteractionMode.Disabled;
                _this.DigiTransitMapControl.RotateInteractionMode = MapInteractionMode.Disabled;
                _this.DigiTransitMapControl.TiltInteractionMode = MapInteractionMode.Disabled;
                if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.Maps.MapControl", "AllowFocusOnInteraction"))
                {
                    _this.DigiTransitMapControl.AllowFocusOnInteraction = false;
                }
            }
        }
        public bool IsInteractionEnabled
        {
            get { return (bool)GetValue(IsInteractionEnabledProperty); }
            set { SetValue(IsInteractionEnabledProperty, value); }
        }

        //Updates the mapcontrol via x:bind binding
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(DigiTransitMap), new PropertyMetadata(10.0));
        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        public DigiTransitMap()
        {
            this.InitializeComponent();

            //Default location of Helsinki's Rautatientori
            DigiTransitMapControl.Center = new Geopoint(new BasicGeoposition { Altitude = 0.0, Latitude = 60.1709, Longitude = 24.9413 });

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            this.Unloaded += DigiTransitMap_Unloaded;            
            this.DigiTransitMapControl.CenterChanged += DigiTransitMapControl_CenterChanged;
            this.DigiTransitMapControl.MapTapped += DigiTransitMapControl_MapTapped;
            this.DigiTransitMapControl.MapRightTapped += DigiTransitMapControl_MapRightTapped;

            _geolocationService = SimpleIoc.Default.GetInstance<IGeolocationService>();

            MapServiceToken = MapHelper.MapApiToken.Value;

            if (_themeIconSource == null)
            {
                _themeIconSource = CircleMapIconSource.GenerateIconAsync(CircleMapIconSource.IconType.ThemeColor);
            }

            if (_greyIconSource == null)
            {
                _greyIconSource = CircleMapIconSource.GenerateIconAsync(CircleMapIconSource.IconType.GreyedOut);
            }

            if (_themeIconPointerOverSource == null)
            {
                _themeIconPointerOverSource = CircleMapIconSource.GenerateIconAsync(CircleMapIconSource.IconType.ThemeColorPointerOver);
            }

            if (_themeIconSelectedSource == null)
            {
                _themeIconSelectedSource = CircleMapIconSource.GenerateIconAsync(CircleMapIconSource.IconType.ThemeColorSelected);
            }
        }        

        private void DigiTransitMap_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_liveUpdateToken != null)
            {
                StopLiveUpdates();
            }
            var places = Places as INotifyCollectionChanged;
            if (places != null)
            {
                places.CollectionChanged -= OnPlacesCollectionChanged;
            }
            var mapLines = ColoredMapLines as INotifyCollectionChanged;
            if(mapLines != null)
            {
                mapLines.CollectionChanged -= OnColoredMapLinePointsCollectionChanged;
            }
            var mapCircles = ColoredCircles as INotifyCollectionChanged;
            if(mapCircles != null)
            {
                mapCircles.CollectionChanged -= OnColoredCirclesCollectionChanged;
            }
            this.DigiTransitMapControl.CenterChanged -= DigiTransitMapControl_CenterChanged;
            this.DigiTransitMapControl.MapTapped -= DigiTransitMapControl_MapTapped;
            this.DigiTransitMapControl.MapRightTapped -= DigiTransitMapControl_MapRightTapped;
            Bindings.StopTracking();            
        }

        private void DigiTransitMapControl_CenterChanged(MapControl sender, object args)
        {
            MapCenterChanged?.Invoke(sender, args);
        }

        private void DigiTransitMapControl_MapRightTapped(MapControl sender, MapRightTappedEventArgs args)
        {
            MapRightTapped?.Invoke(sender, args);
        }

        private void DigiTransitMapControl_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("MapControl tapped.");
            MapTapped?.Invoke(sender, args);
        }

        private void DigiTransitMapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            MapElementClicked?.Invoke(sender, args);
        }
        
        private void DigiTransitMapControl_MapElementPointerEntered(MapControl sender, MapElementPointerEnteredEventArgs args)
        {
            MapIcon icon = args.MapElement as MapIcon;
            if (icon == null)
            {
                return;
            }

            if ((MapIconState)icon.GetValue(MapIconStateProperty) == MapIconState.None)
            {
                // The MapIconChanged callback in the Attached Property handles Image changing on State changes. See MapElementExtensions.cs.
                icon.SetValue(MapIconStateProperty, MapIconState.PointerOver);
            }
        }

        private void DigiTransitMapControl_MapElementPointerExited(MapControl sender, MapElementPointerExitedEventArgs args)
        {
            MapIcon icon = args.MapElement as MapIcon;
            if (icon == null)
            {
                return;
            }

            if ((MapIconState)icon.GetValue(MapIconStateProperty) == MapIconState.PointerOver)
            {
                // The MapIconChanged callback in the Attached Property handles Image changing on State changes. See MapElementExtensions.cs.
                icon.SetValue(MapIconStateProperty, MapIconState.None);
            }
        }

        private void AddMapLines(IEnumerable<MapPolyline> polylines)
        {
            foreach (var newLine in polylines)
            {
                DigiTransitMapControl.MapElements.Add(newLine);
            }

            this?.MapElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveMapLines(IEnumerable<MapPolyline> polylines)
        {
            foreach(var removedLine in polylines)
            {
                DigiTransitMapControl.MapElements.Remove(removedLine);
            }

            this?.MapElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetMapLines(IEnumerable<MapPolyline> polylines)
        {
            var oldList = DigiTransitMapControl.MapElements.OfType<MapPolyline>().ToList();
            if (polylines == null)
            {
                if(oldList != null)
                {
                    RemoveMapLines(oldList);
                }
                return;
            }

            if (oldList != null)
            {
                var toAdd = polylines.Except(oldList);
                AddMapLines(toAdd);
            }
            else
            {
                var toAdd = polylines;
                AddMapLines(toAdd);
            }

            if (oldList != null)
            {
                var toRemove = oldList.Except(polylines);
                RemoveMapLines(toRemove);
            }
        }

        private void AddMapIcons(IEnumerable<MapIcon> icons)
        {
            foreach (var newIcon in icons)
            {
                DigiTransitMapControl.MapElements.Add(newIcon);
            }
            MapElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveMapIcons(IEnumerable<MapIcon> icons)
        {
            foreach(var oldIcon in icons)
            {
                DigiTransitMapControl.MapElements.Remove(oldIcon);
            }
            MapElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetMapIcons(IEnumerable<MapIcon> icons)
        {
            List<MapIcon> oldList = DigiTransitMapControl.MapElements.OfType<MapIcon>().ToList();
            if (icons == null)
            {
                if(oldList != null)
                {
                    RemoveMapIcons(oldList);
                }
                return;
            }

            if (oldList != null)
            {
                AddMapIcons(icons.Except(oldList));
            }
            else
            {
                AddMapIcons(icons);
            }

            if (oldList != null)
            {
                RemoveMapIcons(oldList.Except(icons));
            }
        }

        private void AddMapPolygons(IEnumerable<MapPolygon> polygons)
        {
            foreach(var newPolygon in polygons)
            {
                DigiTransitMapControl.MapElements.Add(newPolygon);
            }
            MapElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveMapPolygons(IEnumerable<MapPolygon> polygons)
        {
            foreach(var oldPolygon in polygons)
            {
                DigiTransitMapControl.MapElements.Remove(oldPolygon);
            }
            MapElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetMapPolygons(IEnumerable<MapPolygon> polygons)
        {
            var oldList = DigiTransitMapControl.MapElements.OfType<MapPolygon>().ToList();
            if (polygons == null)
            {
                if (oldList != null)
                {
                    RemoveMapPolygons(oldList);
                }
                return;
            }

            if(oldList != null)
            {
                AddMapPolygons(polygons.Except(oldList));
            }
            else
            {
                AddMapPolygons(polygons);
            }

            if(oldList != null)
            {
                RemoveMapPolygons(oldList.Except(polygons));
            }
        }

        private void StartLiveUpdates()
        {
            _liveUpdateToken = _geolocationService.BeginLiveUpdates();
            _geolocationService.PositionChanged += GeopositionChanged;
            _geolocationService.HeadingChanged += HeadingChanged;
        }

        private void StopLiveUpdates()
        {
            _liveUpdateToken.Dispose();
            _liveUpdateToken = null;
            _geolocationService.PositionChanged -= GeopositionChanged;
            _geolocationService.HeadingChanged -= HeadingChanged;
        }

        private void GeopositionChanged(PositionChangedEventArgs args)
        {
            _lastKnownGeopoint = args.Position.Coordinate.Point;
            if (ShowUserOnMap)
            {
                if(this.SelfMarker.Visibility == Visibility.Collapsed)
                {
                    this.SelfMarker.Visibility = Visibility.Visible;
                    MapControl.SetNormalizedAnchorPoint(this.SelfMarker, new Point(MapSelfMarker.RenderTransformOriginX, MapSelfMarker.RenderTransformOriginY));
                }
                MapControl.SetLocation(SelfMarker, args.Position.Coordinate.Point);
            }
        }

        private void HeadingChanged(CompassReadingChangedEventArgs args)
        {
            _lastKnownHeading = args.Reading;
            if(ShowUserOnMap)
            {                
                SelfMarker.IsArrowVisible = true;
                SelfMarker.RotationDegrees = args.Reading.HeadingMagneticNorth;
            }
        }

        public async Task TrySetViewBoundsAsync(GeoboundingBox bounds, Thickness? margin, MapAnimationKind animation, bool retryOnFailure = false)
        {
            if(DigiTransitMapControl == null)
            {
                return;
            }

            if(margin != null && DeviceTypeHelper.GetDeviceFormFactorType() != DeviceFormFactorType.Phone)
            {
                //Margins are a little smaller on desktop for some reason. investigate this a little further, may just be a DPI thing?
                const int desktopPlusCoeff = 40;
                margin = new Thickness(margin.Value.Left + desktopPlusCoeff, margin.Value.Top + desktopPlusCoeff,
                    margin.Value.Right + desktopPlusCoeff, margin.Value.Bottom + desktopPlusCoeff);
            }
            //If map movement fails, keep retrying until we get it right
            bool moved = false;
            do
            {
                moved = await DigiTransitMapControl.TrySetViewBoundsAsync(bounds, margin, animation);
                if (moved)
                {
                    break;
                }
                else
                {
                    await Task.Delay(2000); //looong delay to acommodate slow mobile rendering
                }
            } while (!moved && retryOnFailure);
        }

        public async Task TrySetViewAsync(Geopoint point, double? zoomLevel, MapAnimationKind animation)
        {
            await DigiTransitMapControl.TrySetViewAsync(point, zoomLevel, null, null, animation);
        }

        public GeoboundingBox GetBoundingBoxWithIds(Guid poisId)
        {
            if (DigiTransitMapControl.MapElements == null
                || DigiTransitMapControl.MapElements.Count <= 0
                || !DigiTransitMapControl.MapElements.Any(x => x is MapIcon))
            {
                return null;
            }

            List<BasicGeoposition> coords = new List<BasicGeoposition>();
            var iconsCoords = DigiTransitMapControl.MapElements
                .OfType<MapIcon>()
                .Where(x =>
                    {
                        Guid? id = x.GetValue(PoiIdProperty) as Guid?;
                        if(id == null || id != poisId)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    })
                .Select(x => x.Location.Position);
            coords.AddRange(iconsCoords);

            var lineCoords = DigiTransitMapControl.MapElements
                .OfType<MapPolyline>().
                Where(x =>
                {                    
                    Guid? id = x.GetValue(PoiIdProperty) as Guid?;
                    if (id == null || id != poisId)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                })
                .SelectMany(x => x.Path.Positions);
            coords.AddRange(lineCoords);

            return GetCoordinateGeoboundingBox(coords);
        }

        public GeoboundingBox GetPlacesBoundingBox(IEnumerable<BasicGeoposition> pois)
        {
            if (DigiTransitMapControl.MapElements == null
                || DigiTransitMapControl.MapElements.Count <= 0
                || !DigiTransitMapControl.MapElements.Any(x => x is MapIcon))
            {
                return null;
            }

            var poisInMap = DigiTransitMapControl.MapElements.OfType<MapIcon>().Join(
                    pois,
                    x => x.Location.Position,
                    y => y,
                    (x, y) => x)
                .Select(x => x.Location.Position);

            return GetCoordinateGeoboundingBox(poisInMap);
        }

        public GeoboundingBox GetAllMapElementsBoundingBox()
        {
            if(DigiTransitMapControl?.MapElements == null
                || DigiTransitMapControl.MapElements.Count <= 0)
            {
                return null;
            }

            List<BasicGeoposition> elementGeopositions = new List<BasicGeoposition>();

            var linePositions = DigiTransitMapControl.MapElements
                .OfType<MapPolyline>()
                .Select(x => x.Path.Positions)
                .FirstOrDefault();
            if(linePositions != null)
            {
                elementGeopositions.AddRange(linePositions);
            }

            elementGeopositions.AddRange(
                DigiTransitMapControl.MapElements
                .OfType<MapIcon>()
                .Select(x => x.Location.Position)
            );

            if(!elementGeopositions.Any())
            {
                return null;
            }

            return GetCoordinateGeoboundingBox(elementGeopositions);
        }

        public GeoboundingBox GetMapViewBoundingBox()
        {
            //Only available in AU and later
            if (ApiInformation.IsMethodPresent("Windows.UI.Xaml.Controls.Maps.MapControl", "GetVisibleRegion"))
            {
                Geopath geopath = DigiTransitMapControl.GetVisibleRegion(MapVisibleRegionKind.Full);
                if (geopath == null)
                {
                    return null;
                }
                return GetCoordinateGeoboundingBox(geopath.Positions);
            }
            else //pre-AU
            {
                return GetBounds();
            }
        }

        private GeoboundingBox GetCoordinateGeoboundingBox(IEnumerable<BasicGeoposition> coords)
        {
            var topLeft = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = coords.Min(x => x.Longitude),
                Latitude = coords.Max(x => x.Latitude)
            };
            var bottomRight = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = coords.Max(x => x.Longitude),
                Latitude = coords.Min(x => x.Latitude)
            };

            // This is only here because it SEEMS like in the _incredibly_ rare case that my GeoboundingBox is a point,
            // the whole calculation goes crazy, and you enter up with a .Center in Alaska
            if (topLeft.Equals(bottomRight))
            {
                bottomRight.Latitude -= 0.000001;
            }

            return new GeoboundingBox(topLeft, bottomRight);
        }

        //less-good replacement for pre-AU machines
        //doesn't handle rotated maps. maybe an issue? Maybe just disable rotation anywhere we use this...
        private GeoboundingBox GetBounds()
        {
            if (DigiTransitMapControl.Center.Position.Latitude == 0) { return default(GeoboundingBox); }

            double degreePerPixel = (156543.04 * Math.Cos(DigiTransitMapControl.Center.Position.Latitude * Math.PI / 180)) / (111325 * Math.Pow(2, DigiTransitMapControl.ZoomLevel));

            double mHalfWidthInDegrees = DigiTransitMapControl.ActualWidth * degreePerPixel / 0.9;
            double mHalfHeightInDegrees = DigiTransitMapControl.ActualHeight * degreePerPixel / 1.7;

            double mNorth = DigiTransitMapControl.Center.Position.Latitude + mHalfHeightInDegrees;
            double mWest = DigiTransitMapControl.Center.Position.Longitude - mHalfWidthInDegrees;
            double mSouth = DigiTransitMapControl.Center.Position.Latitude - mHalfHeightInDegrees;
            double mEast = DigiTransitMapControl.Center.Position.Longitude + mHalfWidthInDegrees;

            GeoboundingBox mBounds = new GeoboundingBox(
                new BasicGeoposition()
                {
                    Latitude = mNorth,
                    Longitude = mWest
                },
                new BasicGeoposition()
                {
                    Latitude = mSouth,
                    Longitude = mEast
                });

            return mBounds;
        }

        // TODO: If we're setting an icon's state to PointerOver, maybe make sure it's the only icon in a PointerOver state?
        // Hope that's not too slow.
        // Maybe keep a list of all icons currently in a PointerOver state...
        public void SetIconState(Guid iconId, MapIconState iconState)
        {
            MapIcon matchingIcon = DigiTransitMapControl.MapElements
                .OfType<MapIcon>()
                .FirstOrDefault(x => (Guid)x.GetValue(PoiIdProperty) == iconId);

            if (matchingIcon == null)
            {
                return;
            }

            // The MapIconChanged callback in the Attached Property handles Image changing on State changes. See MapElementExtensions.cs.
            matchingIcon.SetValue(MapIconStateProperty, iconState);


            // Only allow one icon to be highlighted at at time by external callers.
            if (iconState != MapIconState.None)
            {                
                foreach (MapIcon activeIcon in DigiTransitMapControl.MapElements.
                    OfType<MapIcon>()
                    .Where(x => (MapIconState)x.GetValue(MapIconStateProperty) != MapIconState.None
                                && (Guid)x.GetValue(PoiIdProperty) != iconId))
                {
                    activeIcon.SetValue(MapIconStateProperty, MapIconState.None);
                }
            }
        }           
    }
}
