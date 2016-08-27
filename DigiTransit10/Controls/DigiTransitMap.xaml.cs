using DigiTransit10.Models;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using System;
using Windows.UI;
using DigiTransit10.Helpers;
using static DigiTransit10.Models.ApiModels.ApiEnums;
using DigiTransit10.Styles;
using Microsoft.Practices.ServiceLocation;
using DigiTransit10.Services;
using Template10.Common;
using GalaSoft.MvvmLight.Ioc;
using Windows.Devices.Sensors;
using System.Collections.Specialized;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class DigiTransitMap : UserControl
    {
        private readonly IGeolocationService _geolocationService;

        private DispatcherTimer _loadingDelayTimer = new DispatcherTimer();
        private LiveGeolocationToken _liveUpdateToken;
        private CompassReading _lastKnownHeading;
        private Geopoint _lastKnownGeopoint;        

        public event EventHandler MapElementsChanged;

        public static readonly DependencyProperty ShowUserOnMapProperty =
            DependencyProperty.Register("ShowUserOnMap", typeof(bool), typeof(DigiTransitMap), new PropertyMetadata(false,
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
                _this.SelfMarker.Visibility = Visibility.Visible;
                MapControl.SetNormalizedAnchorPoint(_this.SelfMarker, new Point(MapSelfMarker.RenderTransformOriginX, MapSelfMarker.RenderTransformOriginY));
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

        public static readonly DependencyProperty MapLinePointsProperty =
                    DependencyProperty.Register("MapLinePoints", typeof(IEnumerable<BasicGeoposition>), typeof(DigiTransitMap), new PropertyMetadata(null,
                        OnMapLinePointsChanged));
        private static void OnMapLinePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if (_this == null)
            {
                return;
            }

            IEnumerable<BasicGeoposition> newLinePoints = e.NewValue as IEnumerable<BasicGeoposition>;
            if (newLinePoints == null)
            {
                if (_this.DigiTransitMapControl.MapElements.OfType<MapPolyline>().Any())
                {
                    _this.SetMapLines(null);
                }
                return;
            }

            MapPolyline polyline = new MapPolyline();
            polyline.Path = new Geopath(newLinePoints);
            polyline.StrokeColor = Color.FromArgb(128, 0, 0, 200); //half-transparent blue
            polyline.StrokeThickness = 6;

            if (_this.DigiTransitMapControl.MapElements.OfType<MapPolyline>().Any())
            {
                _this.SetMapLines(null);
            }

            _this.SetMapLines(new List<MapPolyline> { polyline });
        }
        /// <summary>
        /// A collection of geopoints, between which a single half-transparent blue line is drawn. Exclusive with <see cref="ColoredMapLinePoints"/>.
        /// </summary>
        public IEnumerable<BasicGeoposition> MapLinePoints
        {
            get { return (IEnumerable<BasicGeoposition>)GetValue(MapLinePointsProperty); }
            set { SetValue(MapLinePointsProperty, value); }
        } 
        
        public static readonly DependencyProperty ColoredMapLinePointsProperty =
            DependencyProperty.Register("ColoredMapLinePoints", typeof(IList<ColoredMapLinePoint>), typeof(DigiTransitMap), new PropertyMetadata(null,
                ColoredMapLinePointsChanged));
        private static void ColoredMapLinePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if (_this == null)
            {
                return;
            }
            IList<ColoredMapLinePoint> newValue = e.NewValue as IList<ColoredMapLinePoint>;
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
            for (int i = 0; i <= newValue.Count() - 2; i++)
            {
                var startPoint = newValue[i];
                var nextPoint = newValue[i + 1];
                currentLinePositions.Add(startPoint.Coordinates);

                Color nextColor = nextPoint.LineColor;
                if (nextPoint == newValue.Last() || startPoint.LineColor != nextColor)
                {
                    MapPolyline polyline = new MapPolyline();
                    polyline.Path = new Geopath(currentLinePositions);
                    polyline.StrokeColor = Color.FromArgb(192, startPoint.LineColor.R, startPoint.LineColor.G, startPoint.LineColor.B);
                    polyline.StrokeDashed = startPoint.IsLineDashed;
                    polyline.StrokeThickness = 6;
                    newPolylines.Add(polyline);

                    currentLinePositions = new List<BasicGeoposition>();
                }                                
            }

            _this.SetMapLines(newPolylines);
        }
        /// <summary>
        /// A collection of geopoints, between which lines are drawn. The line's color is determined by the starting point's <see cref="ColoredMapLinePoint.LineColor"/> property.
        /// </summary>
        public IList<ColoredMapLinePoint> ColoredMapLinePoints
        {
            get { return (IList<ColoredMapLinePoint>)GetValue(ColoredMapLinePointsProperty); }
            set { SetValue(ColoredMapLinePointsProperty, value); }
        }

        public static readonly DependencyProperty PlacesProperty =
            DependencyProperty.Register("Places", typeof(IEnumerable<IMapPoi>), typeof(DigiTransitMap), new PropertyMetadata(null,
                new PropertyChangedCallback(OnPlacesChanged)));
        private static void OnPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
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
            if(newList == null || !newList.Any())
            {
                return;
            }

            List<MapIcon> icons = new List<MapIcon>();
            foreach(var place in newList.OfType<IMapPoi>())
            {
                MapIcon element = new MapIcon();
                element.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                element.Location = new Geopoint(place.Coords);
                element.Title = place.Name;
                element.NormalizedAnchorPoint = new Point(0.5, 1.0);
                icons.Add(element);
            }

            _this.SetMapIcons(icons);
        }
        public IEnumerable<IMapPoi> Places
        {
            get { return (IEnumerable<IMapPoi>)GetValue(PlacesProperty); }
            set { SetValue(PlacesProperty, value); }
        }

        private void OnPlacesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(args.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (args.NewItems != null)
            {
                var newIcons = new List<MapIcon>();
                foreach (IMapPoi place in args.NewItems.OfType<IMapPoi>())
                {
                    var element = new MapIcon();
                    element.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                    element.Location = new Geopoint(place.Coords);
                    element.Title = place.Name;
                    element.NormalizedAnchorPoint = new Point(0.5, 1.0);
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

        public DigiTransitMap()
        {
            this.InitializeComponent();

            //Default location of Helsinki's Rautatientori
            DigiTransitMapControl.Center = new Geopoint(new BasicGeoposition { Altitude = 0.0, Latitude = 60.1709, Longitude = 24.9413 });
            DigiTransitMapControl.ZoomLevel = 10;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            _loadingDelayTimer.Interval = TimeSpan.FromMilliseconds(750);
            _loadingDelayTimer.Tick += _layoutUpdateTimer_Tick;
            this.Loaded += DigiTransitMap_Loaded;            

            _geolocationService = SimpleIoc.Default.GetInstance<IGeolocationService>();
        }

        private void DigiTransitMap_Loaded(object sender, RoutedEventArgs e)
        {
            DigiTransitMapControl.Visibility = Visibility.Visible;            
            _loadingDelayTimer.Start();
        }

        private void _layoutUpdateTimer_Tick(object sender, object e)
        {            
            HideLoadingScreenStoryboard.Begin();
            HideLoadingScreenStoryboard.Completed += HideLoadingScreenStoryboard_Completed;
        }

        private void HideLoadingScreenStoryboard_Completed(object sender, object e)
        {
            _loadingDelayTimer.Tick -= HideLoadingScreenStoryboard_Completed;
            _loadingDelayTimer.Stop();
            DigiTransitMapControl.Opacity = 1;
            LoadingRing.Visibility = Visibility.Collapsed;
        }

        private void SetMapLines(IEnumerable<MapPolyline> polylines)
        {
            var oldList = DigiTransitMapControl.MapElements.OfType<MapPolyline>().ToList();

            if (polylines == null || !polylines.Any())
            {
                return;
            }

            if (oldList != null)
            {
                foreach (var element in polylines.Except(oldList))
                {
                    DigiTransitMapControl.MapElements.Add(element);
                }
            }
            else
            {
                foreach (var element in polylines)
                {
                    DigiTransitMapControl.MapElements.Add(element);
                }
            }

            if (oldList != null)
            {
                foreach (var element in oldList.Except(polylines))
                {
                    DigiTransitMapControl.MapElements.Remove(element);
                }
            }

            MapElementsChanged?.Invoke(this, EventArgs.Empty);
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
            var oldList = DigiTransitMapControl.MapElements.OfType<MapIcon>().ToList();

            if (icons == null || !icons.Any())
            {
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_liveUpdateToken != null)
            {
                StopLiveUpdates();
            }
        }

        public async Task TrySetViewBoundsAsync(GeoboundingBox bounds, Thickness? margin, MapAnimationKind animation, bool retryOnFailure = false)
        {
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

        /// <summary>
        /// Returns a GeoboundingBox around all the MapElements currently on the map, or null if there are none.
        /// </summary>
        /// <returns></returns>
        public GeoboundingBox GetMapIconsBoundingBox()
        {
            if (DigiTransitMapControl.MapElements == null 
                || DigiTransitMapControl.MapElements.Count() <= 0 
                || !DigiTransitMapControl.MapElements.Any(x => x is MapIcon))
            {
                return null;
            }

            var topLeft = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = DigiTransitMapControl.MapElements.OfType<MapIcon>()
                                       .Min(x => x.Location.Position.Longitude),
                Latitude = DigiTransitMapControl.MapElements.OfType<MapIcon>()
                                      .Max(x => x.Location.Position.Latitude)
            };

            var bottomRight = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = DigiTransitMapControl.MapElements.OfType<MapIcon>()
                                       .Max(x => x.Location.Position.Longitude),
                Latitude = DigiTransitMapControl.MapElements.OfType<MapIcon>()
                                      .Min(x => x.Location.Position.Latitude)
            };

            return new GeoboundingBox(topLeft, bottomRight);
        }

        public GeoboundingBox GetBoundingBoxForPois(IEnumerable<BasicGeoposition> pois)
        {
            if (DigiTransitMapControl.MapElements == null
                || DigiTransitMapControl.MapElements.Count() <= 0
                || !DigiTransitMapControl.MapElements.Any(x => x is MapIcon))
            {
                return null;
            }

            var poisInMap = DigiTransitMapControl.MapElements.OfType<MapIcon>().Join(pois,
                x =>  new BasicGeoposition { Altitude = 0.0, Latitude = x.Location.Position.Latitude, Longitude = x.Location.Position.Longitude },
                y =>  y,
                (x, y) => x);

            var topLeft = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = poisInMap.Min(x => x.Location.Position.Longitude),
                Latitude = poisInMap.Max(x => x.Location.Position.Latitude)
            };
            var bottomRight = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = poisInMap.Max(x => x.Location.Position.Longitude),
                Latitude = poisInMap.Min(x => x.Location.Position.Latitude)
            };

            return new GeoboundingBox(topLeft, bottomRight);
        }

        public GeoboundingBox GetMapViewBoundingBox()
        {
            var geopath = DigiTransitMapControl.GetVisibleRegion(MapVisibleRegionKind.Full);

            var topLeft = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = geopath.Positions.Min(x => x.Longitude),
                Latitude = geopath.Positions.Max(x => x.Latitude)
            };
            var bottomRight = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = geopath.Positions.Max(x => x.Longitude),
                Latitude = geopath.Positions.Min(x => x.Latitude)
            };

            return new GeoboundingBox(topLeft, bottomRight);
        }             
    }
}
