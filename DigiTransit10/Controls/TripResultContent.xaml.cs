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
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripResultContent : UserControl, INotifyPropertyChanged
    {
        private const int TripListStateIndex = 0;
        private const int DetailedStateIndex = 1;
        private readonly VisualState _tripListState;
        private readonly VisualState _detailedTripState;

        private double _mapWidth = Double.NaN;
        public double MapWidth
        {
            get { return _mapWidth; }
            set
            {
                if(_mapWidth != value)
                {
                    _mapWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TripResultViewModel ViewModel => DataContext as TripResultViewModel;        
       
        public TripResultContent()
        {
            this.InitializeComponent();
            _tripListState = TripStateGroup.States[TripListStateIndex];
            _detailedTripState = TripStateGroup.States[DetailedStateIndex];
            VisualStateManager.GoToState(this, _tripListState.Name, false);

            this.DataContextChanged += (s, e) => RaisePropertyChanged(nameof(ViewModel));
            this.Loaded += TripResultContent_Loaded;
            this.Unloaded += TripResultContent_Unloaded;
            Messenger.Default.Register<MessageTypes.ViewPlanDetails>(this, SwitchToDetailedState);
            Messenger.Default.Register<MessageTypes.ViewPlanStrips>(this, SwitchToListState);
        }        

        private void TripResultContent_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.Parent != null)
            {
                (this.Parent as FrameworkElement).SizeChanged += Parent_SizeChanged;
            }
            ClipToBounds();            
        }

        private void TripResultContent_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent != null)
            {
                (this.Parent as FrameworkElement).SizeChanged -= Parent_SizeChanged;
            }
        }

        private void ClipToBounds()
        {
            this.Clip = new Windows.UI.Xaml.Media.RectangleGeometry()
            {
                Rect = new Windows.Foundation.Rect(0, 0, this.ActualWidth, this.ActualHeight)
            };
        }

        private void SwitchToDetailedState(MessageTypes.ViewPlanDetails details)
        {
            MapWidth = this.ActualWidth; //Otherwise MapControl sets its own width, which, in rare cases, is too wide.
            if (TripStateGroup.CurrentState == _tripListState)
            {
                VisualStateManager.GoToState(this, _detailedTripState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, _detailedTripState.Name, false);
            }

            GeoboundingBox iconsBoundingBox = SingleMap.GetAllMapElementsBoundingBox();
            var mapMargin = new Thickness(10, 10, 10, (this.ActualHeight * .66) + 10);
            SingleMap.TrySetViewBoundsAsync(iconsBoundingBox, mapMargin, MapAnimationKind.None, true).DoNotAwait();
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

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipToBounds();
        }

        private void DetailedTripList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ListView list = sender as ListView;
            if (list == null)
            {
                return;
            }

            //Frame clicked point on the map
            TripLeg clickedLeg = (TripLeg)e.ClickedItem;            
            var boundingBox = SingleMap.GetBoundingBoxWithIds(clickedLeg.TemporaryId);
            double bottomMargin = DirectionsFloatingPanel.IsOpen ? DirectionsFloatingPanel.ExpandedHeight + 10 : 10;
            SingleMap.TrySetViewBoundsAsync(boundingBox, new Thickness(10, 10, 10, bottomMargin), MapAnimationKind.Bow).DoNotAwait();

            //Expand intermediate stops of clicked leg
            var element = list.ContainerFromItem(e.ClickedItem);
            var intermediatesControl = element.FindChild<TripDetailListIntermediates>("IntermediateStops");
            if (intermediatesControl != null)
            {
                intermediatesControl.ToggleViewState();
            }
        }

        private void TripResultList_ItemClick(object sender, ItemClickEventArgs e)
        {
            TripItinerary model = e.ClickedItem as TripItinerary;
            if (ViewModel.ShowTripDetailsCommand.CanExecute(model))
            {
                ViewModel.ShowTripDetailsCommand.Execute(model);
            }
        }

        private void TripResultList_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            var itinerary = (e.OriginalSource as FrameworkElement).DataContext as TripItinerary;
            if (itinerary == null)
            {
                itinerary = (e.OriginalSource as FrameworkElement).FindParent<TripPlanStrip.TripPlanStrip>().DataContext as TripItinerary;
            }

            if (item != null)
            {
                MenuFlyout flyout = FlyoutBase.GetAttachedFlyout(item) as MenuFlyout;
                ((MenuFlyoutItem)flyout.Items[0]).CommandParameter = itinerary;
                flyout.ShowAt(this, e.GetPosition(this));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }        
    }
}
