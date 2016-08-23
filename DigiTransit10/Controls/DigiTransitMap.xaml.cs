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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class DigiTransitMap : UserControl
    {
        private DispatcherTimer _loadingDelayTimer = new DispatcherTimer();        

        public event EventHandler MapElementsChanged;

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
            if(newLinePoints == null)
            {
                var existingPolyLine = _this.DigiTransitMapControl.MapElements.OfType<MapPolyline>().FirstOrDefault();
                if (existingPolyLine != null)
                {
                    _this.DigiTransitMapControl.MapElements.Remove(existingPolyLine);
                }
                return;
            }

            MapPolyline polyline = new MapPolyline();
            polyline.Path = new Geopath(newLinePoints);
            polyline.StrokeColor = Color.FromArgb(128, 0, 0, 200); //half-transparent blue
            polyline.StrokeThickness = 6;

            var oldPolyLine = _this.DigiTransitMapControl.MapElements.OfType<MapPolyline>().FirstOrDefault();
            if(oldPolyLine != null)
            {
                _this.DigiTransitMapControl.MapElements.Remove(oldPolyLine);
            }

            _this.DigiTransitMapControl.MapElements.Add(polyline);
        }
        public IEnumerable<BasicGeoposition> MapLinePoints
        {
            get { return (IEnumerable<BasicGeoposition>)GetValue(MapLinePointsProperty); }
            set { SetValue(MapLinePointsProperty, value); }
        }

        public static readonly DependencyProperty MapElementsProperty =
            DependencyProperty.Register("MapElements", typeof(IEnumerable<MapElement>), typeof(DigiTransitMap), new PropertyMetadata(null,
                new PropertyChangedCallback(OnMapElementsChanged)));
        private static void OnMapElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if(_this == null)
            {
                return;
            }

            var oldList = e.OldValue as IEnumerable<MapElement>;
            var newList = e.NewValue as IEnumerable<MapElement>;
            if(newList == null || !newList.Any())
            {
                return;
            }

            if (oldList != null)
            {
                foreach (var element in newList.Except(oldList))
                {
                    _this.DigiTransitMapControl.MapElements.Add(element);
                }
            }
            else
            {
                foreach (var element in newList)
                {
                    _this.DigiTransitMapControl.MapElements.Add(element);
                }
            }

            if (oldList != null)
            {
                foreach (var element in oldList.Except(newList))
                {
                    _this.DigiTransitMapControl.MapElements.Remove(element);
                }
            }

            _this.MapElementsChanged?.Invoke(_this, EventArgs.Empty);
        }
        public IEnumerable<MapElement> MapElements
        {
            get { return (IEnumerable<MapElement>)GetValue(MapElementsProperty); }
            set { SetValue(MapElementsProperty, value); }
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

            var newList = e.NewValue as IEnumerable<IMapPoi>;
            if(newList == null || !newList.Any())
            {
                return;
            }

            List<MapElement> elements = new List<MapElement>();
            foreach(var place in newList.OfType<IMapPoi>())
            {
                MapIcon element = new MapIcon();
                element.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                element.Location = new Geopoint(place.Coords);
                element.Title = place.Name;
                element.NormalizedAnchorPoint = new Point(0.5, 1.0);
                elements.Add(element);
            }
            _this.MapElements = elements;
        }
        public IEnumerable<IMapPoi> Places
        {
            get { return (IEnumerable<IMapPoi>)GetValue(PlacesProperty); }
            set { SetValue(PlacesProperty, value); }
        }

        public DigiTransitMap()
        {
            this.InitializeComponent();
            _loadingDelayTimer.Interval = TimeSpan.FromMilliseconds(750);
            _loadingDelayTimer.Tick += _layoutUpdateTimer_Tick;
            this.Loaded += DigiTransitMap_Loaded;

            //Default location of Helsinki's Rautatientori
            DigiTransitMapControl.Center = new Geopoint(new BasicGeoposition {Altitude = 0.0, Latitude = 60.1709, Longitude = 24.9413 });
            DigiTransitMapControl.ZoomLevel = 10;
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

        public async Task TrySetViewBoundsAsync(GeoboundingBox bounds, Thickness? margin, MapAnimationKind animation)
        {
            //If map movement fails, keep retrying until we get it right
            bool moved = false;
            while (!moved)
            {
                System.Diagnostics.Debug.WriteLine("Map position attmept...");                
                moved = await DigiTransitMapControl.TrySetViewBoundsAsync(bounds, margin, animation);
                if(moved)
                {
                    break;
                }
                else
                {
                    await Task.Delay(2000); //looong delay to acommodate slow mobile rendering
                }
            }
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
            if (MapElements == null || MapElements.Count() <= 0 || !MapElements.Any(x => x is MapIcon))
            {
                return null;
            }

            var topLeft = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = MapElements.Select(x => x as MapIcon)
                                       .Min(x => x.Location.Position.Longitude),
                Latitude = MapElements.Select(x => x as MapIcon)
                                      .Max(x => x.Location.Position.Latitude)
            };

            var bottomRight = new BasicGeoposition
            {
                Altitude = 0.0,
                Longitude = MapElements.Select(x => x as MapIcon)
                                       .Max(x => x.Location.Position.Longitude),
                Latitude = MapElements.Select(x => x as MapIcon)
                                      .Min(x => x.Location.Position.Latitude)
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
