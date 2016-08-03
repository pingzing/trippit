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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class DigiTransitMap : UserControl
    {
        private DispatcherTimer _layoutUpdateTimer = new DispatcherTimer();

        public event EventHandler MapElementsChanged;

        public static readonly DependencyProperty MapElementsProperty =
            DependencyProperty.Register("MapElements", typeof(List<MapElement>), typeof(DigiTransitMap), new PropertyMetadata(null,
                new PropertyChangedCallback(OnMapElementsChanged)));
        private static void OnMapElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if(_this == null)
            {
                return;
            }

            var oldList = e.OldValue as List<MapElement>;
            var newList = e.NewValue as List<MapElement>;
            if(newList == null || newList.Count == 0)
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
        public List<MapElement> MapElements
        {
            get { return (List<MapElement>)GetValue(MapElementsProperty); }
            set { SetValue(MapElementsProperty, value); }
        }

        public static readonly DependencyProperty PlacesProperty =
            DependencyProperty.Register("Places", typeof(List<IPlace>), typeof(DigiTransitMap), new PropertyMetadata(null,
                new PropertyChangedCallback(OnPlacesChanged)));
        private static void OnPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigiTransitMap _this = d as DigiTransitMap;
            if(_this == null)
            {
                return;
            }
            
            var newList = e.NewValue as IList<IFavorite>;
            if(newList == null || newList.Count == 0)
            {
                return;
            }

            List<MapElement> elements = new List<MapElement>();
            foreach(var place in newList.OfType<IPlace>())
            {
                MapIcon element = new MapIcon();
                element.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                element.Location = new Geopoint(new BasicGeoposition { Altitude = 0.0, Latitude = place.Lat, Longitude = place.Lon });
                element.Title = place.Name;
                element.NormalizedAnchorPoint = new Point(0.5, 1.0);
                elements.Add(element);
            }
            _this.MapElements = elements;
        }
        public List<IPlace> Places
        {
            get { return (List<IPlace>)GetValue(PlacesProperty); }
            set { SetValue(PlacesProperty, value); }
        }
                
        public DigiTransitMap()
        {
            this.InitializeComponent();
            _layoutUpdateTimer.Interval = TimeSpan.FromMilliseconds(750);
            _layoutUpdateTimer.Tick += _layoutUpdateTimer_Tick;
            this.Loaded += DigiTransitMap_Loaded;

            //Default location of Helsinki's Rautatientori
            DigiTransitMapControl.Center = new Geopoint(new BasicGeoposition {Altitude = 0.0, Latitude = 60.1709, Longitude = 24.9413 });
            DigiTransitMapControl.ZoomLevel = 10;                        
        }

        private void DigiTransitMap_Loaded(object sender, RoutedEventArgs e)
        {
            DigiTransitMapControl.Visibility = Visibility.Visible;
            DigiTransitMapControl.LayoutUpdated += DigiTransitMapControl_LayoutUpdated;
        }

        private void DigiTransitMapControl_LayoutUpdated(object sender, object e)
        {
            _layoutUpdateTimer.Stop();
            _layoutUpdateTimer.Start();
        }

        private void _layoutUpdateTimer_Tick(object sender, object e)
        {
            DigiTransitMapControl.LayoutUpdated -= DigiTransitMapControl_LayoutUpdated;
            HideLoadingScreenStoryboard.Begin();
            HideLoadingScreenStoryboard.Completed += HideLoadingScreenStoryboard_Completed;
        }

        private void HideLoadingScreenStoryboard_Completed(object sender, object e)
        {
            _layoutUpdateTimer.Tick -= HideLoadingScreenStoryboard_Completed;
            _layoutUpdateTimer.Stop();            
            DigiTransitMapControl.Opacity = 1;
            LoadingRing.Visibility = Visibility.Collapsed;                        
        }        

        public async Task TrySetViewBoundsAsync(GeoboundingBox bounds, Thickness? margin, MapAnimationKind animation)
        {
            await DigiTransitMapControl.TrySetViewBoundsAsync(bounds, margin, animation);
        }

        public async Task TrySetViewAsync(Geopoint point, double? zoomLevel, MapAnimationKind animation)
        {            
            await DigiTransitMapControl.TrySetViewAsync(point, zoomLevel, null, null, animation);            
        }

        /// <summary>
        /// Returns a GeoboundingBox around all the MapElements currently on the map, or null if there are none.
        /// </summary>
        /// <returns></returns>
        public GeoboundingBox GetMapElementsBoundingBox()
        {
            if (MapElements == null || MapElements.Count <= 0 || !MapElements.Any(x => x is MapIcon))
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
    }
}
