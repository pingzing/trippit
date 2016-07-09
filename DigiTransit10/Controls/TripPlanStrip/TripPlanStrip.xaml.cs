using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls.TripPlanStrip
{
    public sealed partial class TripPlanStrip : UserControl, INotifyPropertyChanged
    {
        private const string RectangleHeightKey = "TripPlanStripRectangleHeight";

        private ObservableCollection<TripLegViewModel> _tripLegs = new ObservableCollection<TripLegViewModel>();
        public ObservableCollection<TripLegViewModel> TripLegs
        {
            get { return _tripLegs; }
            set
            {
                if(_tripLegs != value)
                {
                    _tripLegs = value;
                    RaisePropertyChanged();
                }
            }
        }

        public static readonly DependencyProperty StripItineraryProperty =
            DependencyProperty.Register("StripItinerary", typeof(ApiItinerary), typeof(TripPlanStrip), new PropertyMetadata(null, 
                (obj, args) => {
                    TripPlanStrip thisPlanStrip = obj as TripPlanStrip;
                    if(thisPlanStrip == null)
                    {
                        return;                        
                    }

                    if(args.NewValue == null)
                    {
                        return;
                    }
                    ApiItinerary newPlan = args.NewValue as ApiItinerary;
                    if(newPlan == null)
                    {
                        throw new ArgumentException($"The {nameof(StripItinerary)} property must by of type {nameof(ApiItinerary)}");
                    }

                    thisPlanStrip.CreateNewPlanStrip(newPlan);
                }));


        public static readonly DependencyProperty StartingPlaceNameProperty =
            DependencyProperty.Register("StartingPlaceName", typeof(string), typeof(TripPlanStrip), new PropertyMetadata(AppResources.TripPlanStrip_StartingPlaceDefault));
        public string StartingPlaceName
        {
            get { return (string)GetValue(StartingPlaceNameProperty); }
            set { SetValue(StartingPlaceNameProperty, value); }
        }

        public static readonly DependencyProperty EndingPlaceNameProperty =
            DependencyProperty.Register("EndingPlaceName", typeof(string), typeof(TripPlanStrip), new PropertyMetadata(AppResources.TripPlanStrip_EndPlaceDefault));
        public string EndingPlaceName
        {
            get { return (string)GetValue(EndingPlaceNameProperty); }
            set { SetValue(EndingPlaceNameProperty, value); }
        }               

        public ApiItinerary StripItinerary
        {
            get { return (ApiItinerary)GetValue(StripItineraryProperty); }
            set { SetValue(StripItineraryProperty, value); }
        }        

        public TripPlanStrip()
        {
            this.InitializeComponent();
        }

        private void CreateNewPlanStrip(ApiItinerary thisItinerary)
        {
            TripLegs.Clear();
            loadedGrids.Clear();
            for(int i = 0; i < thisItinerary.Legs.Count; i++)
            {
                bool isEnd = i == thisItinerary.Legs.Count - 1;
                ApiLeg nextLeg = null;
                if(!isEnd)
                {
                    nextLeg = thisItinerary.Legs[i + 1];
                }
                TripLegs.Add(new TripLegViewModel
                {
                    BackingLeg = thisItinerary.Legs[i],
                    StartPlaceName = i == 0 ? StartingPlaceName : thisItinerary.Legs[i].From.Name,
                    EndPlaceName = EndingPlaceName,
                    IsStart = i == 0,
                    IsEnd = isEnd,                    
                    NextLeg = nextLeg
                });
            }
        }        

        List<Grid> loadedGrids = new List<Grid>();
        private void LegContentRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if(grid == null)
            {
                return;
            }
            loadedGrids.Add(grid);
            if(loadedGrids.Count == _tripLegs.Count)
            {
                RepositionIcons(loadedGrids);
            }            
        }

        private void RepositionIcons(List<Grid> loadedGrids)
        {
            foreach (var grid in loadedGrids)
            {
                var firstPoint = (grid.Children.FirstOrDefault() as TripPlanPoint)
                    ?.TripPlanPointRootLayout?.Children
                    ?.Skip(2)?.Take(1)
                    ?.FirstOrDefault() as Ellipse;
                if (firstPoint == null)
                {
                    return;
                }

                var potentialEndpoint = (grid.Children.LastOrDefault() as TripPlanPoint);
                Ellipse thirdPoint = null;
                if (potentialEndpoint.Visibility != Visibility.Collapsed)
                {
                    thirdPoint = potentialEndpoint?.TripPlanPointRootLayout?
                        .Children
                        ?.Skip(2)?.Take(1)
                        ?.FirstOrDefault() as Ellipse;
                }                
                
                if (thirdPoint == null)
                {
                    var currentVm = grid.Tag as TripLegViewModel;
                    var targetVm = _tripLegs[_tripLegs.IndexOf(currentVm) + 1];
                    var newGrid = loadedGrids.Where(x => x.Tag == targetVm).FirstOrDefault();
                    if(newGrid == null)
                    {
                        return;
                    }
                    thirdPoint = (newGrid.Children.FirstOrDefault() as TripPlanPoint)
                        ?.TripPlanPointRootLayout?.Children
                        ?.Skip(2)?.Take(1)
                        ?.FirstOrDefault() as Ellipse;
                }
                
                if (thirdPoint == null)
                {
                    return;
                }

                var icon = (grid.Children.Skip(1).Take(1).First() as TripPlanTransitIcon);

                double firstPointCenterX = firstPoint.TransformToVisual(Window.Current.Content)
                    .TransformPoint(new Point(0, 0)).X + (double)this.Resources[RectangleHeightKey];
                double iconCenterX = icon.TransformToVisual(Window.Current.Content)
                    .TransformPoint(new Point(0, 0)).X + (icon.ActualWidth / 2);
                double thirdPointCenterX = thirdPoint.TransformToVisual(Window.Current.Content)
                    .TransformPoint(new Point(0, 0)).X + (double)this.Resources[RectangleHeightKey];
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

    public class TripLegViewModel
    {
        public bool IsEnd { get; set; }
        public bool IsStart { get; set; }
        public string StartPlaceName { get; set; }
        public string EndPlaceName { get; set; }
        public ApiLeg BackingLeg { get; set; }
        public ApiLeg NextLeg { get; set; }                        
    }
}
