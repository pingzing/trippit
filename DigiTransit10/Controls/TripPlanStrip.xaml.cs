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

namespace DigiTransit10.Controls
{
    public sealed partial class TripPlanStrip : UserControl, INotifyPropertyChanged
    {
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
                    IsStart = i == 0,
                    IsEnd = isEnd,
                    NextLeg = nextLeg
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
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
                    ?.Skip(1)?.Take(1)
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
                        ?.Skip(1)?.Take(1)
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
                        ?.Skip(1)?.Take(1)
                        ?.FirstOrDefault() as Ellipse;
                }
                
                if (thirdPoint == null)
                {
                    return;
                }

                var icon = (grid.Children.Skip(1).Take(1).First() as TripPlanTransitMethod);

                double firstPointCenterX = firstPoint.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).X + 10;
                double iconCenterX = icon.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).X + (icon.ActualWidth / 2);
                double thirdPointCenterX = thirdPoint.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).X + 10;                
                double circlesMidpoint = (firstPointCenterX + thirdPointCenterX) / 2;

                double difference = circlesMidpoint - iconCenterX;
                icon.RenderTransform = new TranslateTransform { X = difference };                                          
            }
        }
    }

    public class TripLegViewModel
    {
        public bool IsEnd { get; set; }
        public bool IsStart { get; set; }
        public ApiLeg BackingLeg { get; set; }
        public ApiLeg NextLeg { get; set; }
        public HorizontalAlignment LegAlignment
        {
            get
            {
                if(IsStart)
                {
                    return HorizontalAlignment.Left;
                }
                else
                {
                    return HorizontalAlignment.Center;
                }
            }
        }

        //We want to the make the line for a final Leg draw all the way to the end.
        public int LineColumnSpan => IsEnd ? 3 : 2;
        //Adjust the margin to not overdraw past the circle for an End Leg.
        public Thickness LineMargin => IsEnd
            ? new Thickness(-5, 0, 5, 0)
            : new Thickness(5, 0, -5, 0);
    }
}
