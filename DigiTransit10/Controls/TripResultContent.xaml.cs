using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using DigiTransit10.Helpers;
using System;
using Windows.Devices.Geolocation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripResultContent : UserControl, INotifyPropertyChanged
    {
        private const int TripListStateIndex = 0;
        private const int DetailedStateIndex = 1;
        private readonly VisualState _tripListState;
        private readonly VisualState _detailedTripState;

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
            this.Loaded += TripResultContent_Loaded;
            Messenger.Default.Register<MessageTypes.ViewPlanDetails>(this, SwitchToDetailedState);
            Messenger.Default.Register<MessageTypes.ViewPlanStrips>(this, SwitchToListState);
        }

        private void ClipToBounds()
        {
            this.Clip = new Windows.UI.Xaml.Media.RectangleGeometry()
            {
                Rect = new Windows.Foundation.Rect(0, 0, this.ActualWidth, this.ActualHeight)
            };
        }

        private async void SwitchToDetailedState(MessageTypes.ViewPlanDetails details)
        {
            if (TripStateGroup.CurrentState == _tripListState)
            {
                VisualStateManager.GoToState(this, _detailedTripState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, _detailedTripState.Name, false);
            }

            GeoboundingBox iconsBoundingBox = SingleMap.GetMapIconsBoundingBox();
            var mapMargin = new Thickness(10, 10, 10, (this.ActualHeight * .66) + 10);
            await SingleMap.TrySetViewBoundsAsync(iconsBoundingBox, mapMargin, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
        }

        private void SwitchToListState(MessageTypes.ViewPlanStrips obj)
        {
            if (TripStateGroup.CurrentState == _detailedTripState)
            {
                VisualStateManager.GoToState(this, _tripListState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, _tripListState.Name, false);
            }
        }

        private void TripResultContent_Loaded(object sender, RoutedEventArgs e)
        {
            ClipToBounds();
        }

        private void DirectionsFloatingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DirectionsFloatingPanel.ExpandedHeight = this.ActualHeight * .66;
            this.SizeChanged += TripResultContent_SizeChanged;            
        }

        private void TripResultContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DirectionsFloatingPanel.ExpandedHeight = this.ActualHeight * .66;
            ClipToBounds();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void DetailedTripList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ListView list = sender as ListView;
            if (list == null)
            {
                return;
            }

            //todo: adjust map view
            
            var element = list.ContainerFromItem(e.ClickedItem);            
            var intermediatesControl = element.FindChild<TripDetailListIntermediates>("IntermediateStops");
            if (intermediatesControl != null)
            {
                intermediatesControl.ToggleViewState();
            }
        }

        private void TripResultList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItineraryModel model = e.ClickedItem as ItineraryModel;
            if (ViewModel.ShowTripDetailsCommand.CanExecute(model))
            {
                ViewModel.ShowTripDetailsCommand.Execute(model);
            }
        }
    }
}
