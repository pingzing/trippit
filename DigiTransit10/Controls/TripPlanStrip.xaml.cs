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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripPlanStrip : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<TripLeg> _tripLegs = new ObservableCollection<TripLeg>();
        public ObservableCollection<TripLeg> TripLegs
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
            foreach(var leg in thisItinerary.Legs)
            {
                TripLegs.Add(new TripLeg
                {
                    BackingLeg = leg,
                    IsStart = thisItinerary.Legs.First() == leg,
                    IsEnd = thisItinerary.Legs.Last() == leg
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }        
    }

    public class TripLeg
    {
        public bool IsEnd { get; set; }
        public bool IsStart { get; set; }
        public ApiLeg BackingLeg { get; set; }
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
