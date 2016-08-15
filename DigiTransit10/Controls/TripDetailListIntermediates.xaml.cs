using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripDetailListIntermediates : UserControl
    {
        //Attached via binding, but doesn't need to fire NotifyPropertyChanged, because the ItemsControl that binds to it used a Lazy LoadStrategy.
        private List<ApiStop> _backingStopList = null;
        private bool _isShowingIntermediateStops = false;

        public static readonly DependencyProperty TripLegProperty = DependencyProperty.Register(
            "TripLeg", typeof (DetailedTripListLeg), typeof (TripDetailListIntermediates), new PropertyMetadata(null,
                TripLegChanged));
        private static void TripLegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripDetailListIntermediates _this = d as TripDetailListIntermediates;
            if (_this == null)
            {
                return;
            }

            DetailedTripListLeg newLeg = e.NewValue as DetailedTripListLeg;
            if (newLeg == null)
            {
                return;
            }

            if (newLeg.IntermediateStops?.Count > 0)
            {
                _this._backingStopList = new List<ApiStop>(newLeg.IntermediateStops);
                _this.SimpleTextBlock.Text = $"{newLeg.IntermediateStops.Count} {AppResources.TripDetailListIntermediates_IntermediateStopsNumber}";
            }
            else
            {
                var distanceString = newLeg.Distance.ToString("N0");                    
                _this.SimpleTextBlock.Text = String.Format(AppResources.TripDetailListIntermediates_WalkDistance, distanceString);
            }
        }
        public DetailedTripListLeg TripLeg
        {
            get { return (DetailedTripListLeg) GetValue(TripLegProperty); }
            set { SetValue(TripLegProperty, value); }
        }

        public TripDetailListIntermediates()
        {
            this.InitializeComponent();
        }

        public void ToggleViewState()
        {
            if (!(_backingStopList?.Count > 0))
            {
                return;
            }

            if (IntermediateStopsControl == null)
            {
                FindName(nameof(IntermediateStopsControl));
                IntermediateStopsControl.ItemsSource = _backingStopList;
            }

            if (_isShowingIntermediateStops)
            {
                IntermediateStopsControl.Visibility = Visibility.Collapsed;
                SimpleTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                IntermediateStopsControl.Visibility = Visibility.Visible;
                SimpleTextBlock.Visibility = Visibility.Collapsed;
            }
            _isShowingIntermediateStops = !_isShowingIntermediateStops;
        }
    }
}
