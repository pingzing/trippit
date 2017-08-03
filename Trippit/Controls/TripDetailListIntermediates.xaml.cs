using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Trippit.Localization.Strings;
using Trippit.Models;
using Trippit.Models.ApiModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Controls
{
    public sealed partial class TripDetailListIntermediates : UserControl
    {
        private ObservableCollection<TransitStop> ItemsBackingCollection = new ObservableCollection<TransitStop>();

        private List<TransitStop> _backingIntermediatesList = null;
        private string _backingSimpleText = null;
        private bool _isShowingIntermediateStops = false;

        public static readonly DependencyProperty TripLegProperty = DependencyProperty.Register(
            "TripLeg", typeof (TripLeg), typeof (TripDetailListIntermediates), new PropertyMetadata(null,
                TripLegChanged));
        private static void TripLegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripDetailListIntermediates _this = d as TripDetailListIntermediates;
            if (_this == null)
            {
                return;
            }

            TripLeg newLeg = e.NewValue as TripLeg;
            if (newLeg == null)
            {
                return;
            }

            if (newLeg.IntermediateStops?.Count > 0)
            {
                _this._backingIntermediatesList = newLeg.IntermediateStops;
                _this._backingSimpleText = $"{newLeg.IntermediateStops.Count} {AppResources.TripDetailListIntermediates_IntermediateStopsNumber}";
            }
            else
            {
                var distanceString = newLeg.DistanceMeters.ToString("N0");
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
                _this.ItemsBackingCollection = new ObservableCollection<TransitStop>(_this._backingIntermediatesList);
            }
            else
            {
                TransitStop simpleTextProxy = new TransitStop { Name = _this._backingSimpleText };
                _this.ItemsBackingCollection = new ObservableCollection<TransitStop>();
                _this.ItemsBackingCollection.Add(simpleTextProxy);
            }
            _this.IntermediateStopsControl.ItemsSource = _this.ItemsBackingCollection;
        }
        public TripLeg TripLeg
        {
            get { return (TripLeg) GetValue(TripLegProperty); }
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
                ItemsBackingCollection.Add(new TransitStop { Name = _backingSimpleText });
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
