using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
           this.DataContextChanged += (s, e) => RaisePropertyChanged(nameof(ViewModel));
            Messenger.Default.Register<MessageTypes.ViewPlanDetails>(this, SwitchToDetailedState);            
        }        

        private void SwitchToDetailedState(MessageTypes.ViewPlanDetails obj)
        {
            if (TripStateGroup.CurrentState == TripStateGroup.States[TripListStateIndex]
                || TripStateGroup.CurrentState == null)
            {
                VisualStateManager.GoToState(this, this.TripStateGroup.States[DetailedStateIndex].Name, true);
                DetailedTripList.ItemsSource = obj.BackingModel.BackingItinerary.Legs;
            }
            else if(TripStateGroup.CurrentState == TripStateGroup.States[DetailedStateIndex])
            {
                VisualStateManager.GoToState(this, this.TripStateGroup.States[TripListStateIndex].Name, true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async void DirectionsFloatingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DirectionsFloatingPanel.ExpandedHeight = this.ActualHeight * .75;
            DirectionsFloatingPanel.CollapsedHeight = this.ActualHeight * .25;     
            
            //await adjust map view bounds
        }
    }
}
