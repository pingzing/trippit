using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Messaging;
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
using Windows.UI.Xaml.Controls.Maps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritesPage : Page
    {
        public FavoritesViewModel ViewModel => this.DataContext as FavoritesViewModel;

        public FavoritesPage()
        {
            this.InitializeComponent();
        }

        private void Favorite_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView list = sender as ListView;
            IFavorite tappedItem = (IFavorite)((FrameworkElement)e.OriginalSource).DataContext;
            if (list == null)
            {
                return;
            }
            MenuFlyout flyout = FlyoutBase.GetAttachedFlyout(list) as MenuFlyout;
            ((MenuFlyoutItem)flyout.Items[0]).CommandParameter = tappedItem;
            flyout.ShowAt(this, e.GetPosition(this));
        }

        private void FavoritesListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var boundingBox = this.FavoritesMap.GetMapIconsBoundingBox();
            if (boundingBox != null)
            {
                await this.FavoritesMap.TrySetViewBoundsAsync(boundingBox, new Thickness(450, 50, 50, 50), MapAnimationKind.None);
            }
        }
    }
}
