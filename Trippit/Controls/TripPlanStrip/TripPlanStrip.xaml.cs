using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Trippit.ExtensionMethods;
using Trippit.Models;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Controls.TripPlanStrip
{
    public sealed partial class TripPlanStrip : UserControl, INotifyPropertyChanged
    {
        private const string RectangleHeightKey = "TripPlanStripRectangleHeight";
        private readonly double RectangleHeight;
        
        private ObservableCollection<TripLeg> _tripLegs = new ObservableCollection<TripLeg>();
        public ObservableCollection<TripLeg> TripLegs
        {
            get { return _tripLegs; }
            set
            {
                if (_tripLegs != value)
                {
                    _tripLegs = value;
                    RaisePropertyChanged();
                }
            }
        }

        public static readonly DependencyProperty StripItineraryProperty =
            DependencyProperty.Register("StripItinerary", typeof(TripItinerary), typeof(TripPlanStrip), new PropertyMetadata(null,
                (obj, args) =>
                {
                    TripPlanStrip thisPlanStrip = obj as TripPlanStrip;
                    if (thisPlanStrip == null)
                    {
                        return;
                    }

                    if (args.NewValue == null)
                    {
                        return;
                    }
                    TripItinerary newPlan = args.NewValue as TripItinerary;
                    if (newPlan == null)
                    {
                        throw new ArgumentException($"The {nameof(StripItinerary)} property must by of type {nameof(TripItinerary)}");
                    }

                    thisPlanStrip.CreateNewPlanStrip(newPlan);
                }));

        public TripItinerary StripItinerary
        {
            get { return (TripItinerary)GetValue(StripItineraryProperty); }
            set { SetValue(StripItineraryProperty, value); }
        }

        public TripPlanStrip()
        {
            this.InitializeComponent();
            RectangleHeight = (double)App.Current.Resources[RectangleHeightKey];            
        }

        private void CreateNewPlanStrip(TripItinerary thisItinerary)
        {
            TripLegs.Clear();
            loadedGrids.Clear();
            foreach(TripLeg leg in thisItinerary.ItineraryLegs)
            {
                TripLegs.Add(leg);
            }
        }

        List<Grid> loadedGrids = new List<Grid>();
        private void LegContentRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid == null)
            {
                return;
            }
            loadedGrids.Add(grid);
            if (loadedGrids.Count == _tripLegs.Count)
            {
                RepositionIcons(loadedGrids);
            }
        }

        private void RepositionIcons(List<Grid> loadedGrids)
        {

            foreach (var grid in loadedGrids)
            {
                var firstPoint = (grid.GetNthGridChildOrNull(0) as TripPlanPoint)
                    ?.TripPlanPointRootLayout?.GetNthGridChildOrNull(2) as Ellipse;                    
                if (firstPoint == null)
                {
                    return;
                }

                var potentialEndpoint = (grid.GetNthGridChildOrNull(2) as TripPlanPoint);
                Ellipse thirdPoint = null;
                if (potentialEndpoint?.Visibility != Visibility.Collapsed)
                {
                    thirdPoint = potentialEndpoint
                        ?.TripPlanPointRootLayout
                        ?.GetNthGridChildOrNull(2) as Ellipse;
                }

                if (thirdPoint == null)
                {
                    var currentVm = grid.Tag as TripLeg;
                    var targetVm = _tripLegs[_tripLegs.IndexOf(currentVm) + 1];
                    var newGrid = loadedGrids.FirstOrDefault(x => x.Tag == targetVm);
                    if (newGrid == null)
                    {
                        return;
                    }
                    thirdPoint = (newGrid.GetNthGridChildOrNull(0) as TripPlanPoint)
                        ?.TripPlanPointRootLayout
                        ?.GetNthGridChildOrNull(2) as Ellipse;
                }

                if (thirdPoint == null)
                {
                    return;
                }

                var icon = (grid.GetNthGridChildOrNull(1) as TripPlanTransitIcon);
                var currWindow = Window.Current.Content;
                Point emptyPoint = new Point(0, 0);

                double firstPointCenterX = firstPoint.TransformToVisual(currWindow)
                    .TransformPoint(emptyPoint).X + RectangleHeight;
                double iconCenterX = icon.TransformToVisual(currWindow)
                    .TransformPoint(emptyPoint).X + (icon.ActualWidth / 2);
                double thirdPointCenterX = thirdPoint.TransformToVisual(currWindow)
                    .TransformPoint(emptyPoint).X + RectangleHeight;
                double circlesMidpoint = (firstPointCenterX + thirdPointCenterX) / 2;

                double difference = circlesMidpoint - iconCenterX;
                icon.HorizontalOffset = difference;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }      
}
