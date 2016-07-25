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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Template10.Common;
using DigiTransit10.Helpers;

namespace DigiTransit10.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainViewModel ViewModel => this.DataContext as MainViewModel;        

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.AdaptiveVisualStateGroup.CurrentStateChanged += AdaptiveVisualStateGroup_CurrentStateChanged;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            string currentStateName = AdaptiveVisualStateGroup.CurrentState.Name;
            if (BootStrapper.Current.SessionState.ContainsKey(Constants.CurrentMainPageVisualStateKey))
            {
                BootStrapper.Current.SessionState[Constants.CurrentMainPageVisualStateKey] = currentStateName;
            }
            else
            {
                BootStrapper.Current.SessionState.Add(Constants.CurrentMainPageVisualStateKey, currentStateName);
            }
        }

        private void AdaptiveVisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (BootStrapper.Current.SessionState.ContainsKey(Constants.CurrentMainPageVisualStateKey))
            {
                BootStrapper.Current.SessionState[Constants.CurrentMainPageVisualStateKey] = e.NewState.Name;
            }
            else
            {
                BootStrapper.Current.SessionState.Add(Constants.CurrentMainPageVisualStateKey, e.NewState.Name);
            }
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void PinnedFavoritesControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            FavoritePlace clickedPlace = e.ClickedItem as FavoritePlace;
            if (clickedPlace != null)
            {
                ViewModel?.TripFormViewModel?.FavoritePlaceClickedCommand.Execute(clickedPlace);
            }
        }
    }
}
