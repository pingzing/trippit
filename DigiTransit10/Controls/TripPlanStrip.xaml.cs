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
                    ApiItinerary newItinerary = args.NewValue as ApiItinerary;
                    if(newItinerary == null)
                    {
                        throw new ArgumentException($"The {nameof(StripItinerary)} property must by of type {nameof(ApiItinerary)}");
                    }

                    thisPlanStrip.CreateNewPlanStrip(thisPlanStrip);

                }));

        public ApiItinerary StripItinerary
        {
            get { return (ApiItinerary)GetValue(StripItineraryProperty); }
            set { SetValue(StripItineraryProperty, value); }
        }

        public double HalfStartWidth => this.StartSection.ActualWidth / 2;

        public TripPlanStrip()
        {
            this.InitializeComponent();
        }

        private void CreateNewPlanStrip(TripPlanStrip thisPlanStrip)
        {
            throw new NotImplementedException();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HalfStartWidth));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }        
    }
}
