using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using DigiTransit10.Helpers;
using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Messaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripResultContent : UserControl, INotifyPropertyChanged
    {
        private const int TripListStateIndex = 0;
        private const int DetailedStateIndex = 1;
        private VisualState _tripListState;
        private VisualState _detailedTripState;

        public TripResultViewModel ViewModel => DataContext as TripResultViewModel;

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TripResultContent), new PropertyMetadata(null));
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public TripResultContent()
        {
            this.InitializeComponent();
            _tripListState = TripStateGroup.States[TripListStateIndex];
            _detailedTripState = TripStateGroup.States[DetailedStateIndex];
            VisualStateManager.GoToState(this, _tripListState.Name, false);

            this.DataContextChanged += (s, e) => RaisePropertyChanged(nameof(ViewModel));            
            Messenger.Default.Register<MessageTypes.ViewPlanDetails>(this, SwitchToDetailedState);            
        }        

        private async void SwitchToDetailedState(MessageTypes.ViewPlanDetails obj)
        {
            if (TripStateGroup.CurrentState == _tripListState)
            {                               
                VisualStateManager.GoToState(this, _detailedTripState.Name, true);
                DetailedTripList.ItemsSource = obj.BackingModel.BackingItinerary.Legs;
            }
            else if(TripStateGroup.CurrentState == _detailedTripState)
            {
                
                VisualStateManager.GoToState(this, _tripListState.Name, true);
            }
        }

        private async void DirectionsFloatingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DirectionsFloatingPanel.ExpandedHeight = this.ActualHeight * .66;
            this.SizeChanged += TripResultContent_SizeChanged;

            //await adjust map view bounds
        }

        private void TripResultContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DirectionsFloatingPanel.ExpandedHeight = this.ActualHeight * .66;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }        
    }
}
