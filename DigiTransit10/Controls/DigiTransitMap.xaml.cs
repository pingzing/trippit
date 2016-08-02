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
            
            var newList = e.NewValue as List<IPlace>;
            if(newList == null || newList.Count == 0)
            {
                return;
            }

            List<MapElement> elements = new List<MapElement>();
            foreach(var place in newList)
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
        }

        private void _layoutUpdateTimer_Tick(object sender, object e)
        {
            HideLoadingScreenStoryboard.Begin();
            HideLoadingScreenStoryboard.Completed += HideLoadingScreenStoryboard_Completed;
        }

        private void HideLoadingScreenStoryboard_Completed(object sender, object e)
        {
            _layoutUpdateTimer.Tick -= HideLoadingScreenStoryboard_Completed;
            _layoutUpdateTimer.Stop();
            this.LayoutUpdated -= DigiTransitMapControl_LayoutUpdated;   
                     
            LoadingScreen.Visibility = Visibility.Collapsed;
            LoadingRing.Visibility = Visibility.Collapsed;            
        }

        private void DigiTransitMapControl_LayoutUpdated(object sender, object e)
        {
            _layoutUpdateTimer.Stop();
            _layoutUpdateTimer.Start();
        }
    }
}
