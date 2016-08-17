using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using System.Collections.ObjectModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripDetailListIntermediates : UserControl
    {
        private ObservableCollection<ApiStop> ItemsBackingCollection = new ObservableCollection<ApiStop>();

        private List<ApiStop> _backingIntermediatesList = null;
        private string _backingSimpleText = null;
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
                _this._backingIntermediatesList = new List<ApiStop>(newLeg.IntermediateStops);
                _this._backingSimpleText = $"{newLeg.IntermediateStops.Count} {AppResources.TripDetailListIntermediates_IntermediateStopsNumber}";
            }
            else
            {
                var distanceString = newLeg.Distance.ToString("N0");
                if (newLeg.Mode == ApiEnums.ApiMode.Walk)
                {
                    _this._backingSimpleText = String.Format(AppResources.TripDetailListIntermediates_WalkDistance, distanceString);
                }
                else
                {
                    _this._backingSimpleText = String.Format(AppResources.TripDetailListIntermediates_TransitDistance, distanceString);
                }
            }

            if(_this._isShowingIntermediateStops)
            {
                _this.ItemsBackingCollection = new ObservableCollection<ApiStop>(_this._backingIntermediatesList);
            }
            else
            {
                ApiStop simpleTextProxy = new ApiStop { Name = _this._backingSimpleText };
                _this.ItemsBackingCollection = new ObservableCollection<ApiStop>();
                _this.ItemsBackingCollection.Add(simpleTextProxy);
            }
            _this.IntermediateStopsControl.ItemsSource = _this.ItemsBackingCollection;
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
            if (!(_backingIntermediatesList?.Count > 0))
            {
                return;
            }

            if (_isShowingIntermediateStops)
            {
                ItemsBackingCollection.Clear();
                ItemsBackingCollection.Add(new ApiStop { Name = _backingSimpleText });
            }
            else
            {
                ItemsBackingCollection.Clear();
                foreach (var item in _backingIntermediatesList)
                {
                    ItemsBackingCollection.Add(item);
                }
            }
            _isShowingIntermediateStops = !_isShowingIntermediateStops;
        }
    }
}
