using System;
using DigiTransit10.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using DigiTransit10.Localization.Strings;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using DigiTransit10.Models;

namespace DigiTransit10.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel => this.DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;                          
        }

        private void HideShowOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton button = sender as HyperlinkButton;
            if(button == null)
            {
                return;
            }

            ViewModel?.TripFormViewModel?.ToggleTransitPanelCommand.Execute(null);
        }

        private void PlanTripButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if(AdaptiveVisualStateGroup.CurrentState.Name == "VisualStateNarrow")
            {
                ViewModel.TripFormViewModel.PlanTripNarrowViewCommand.Execute(null);
            }
            else
            {
                ViewModel.TripFormViewModel.PlanTripWideViewCommand.Execute(null);
            }
        }

        private void Favorite_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            var tappedItem = (e.OriginalSource as FrameworkElement).DataContext as IFavorite;
            ShowFavoriteContextMenu(item, e.GetPosition(this), tappedItem);
        }

        private void ShowFavoriteContextMenu(FrameworkElement item, Point point, IFavorite tappedItem)
        {
            if(item != null)
            {
                MenuFlyout flyout = FlyoutBase.GetAttachedFlyout(item) as MenuFlyout;
                ((MenuFlyoutItem)flyout.Items[0]).CommandParameter = tappedItem;
                flyout?.ShowAt(this, point);                
            }
        }
    }
}
