using DigiTransit10.Models;
using DigiTransit10.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Maps;
using DigiTransit10.ExtensionMethods;
using System.Threading.Tasks;
using DigiTransit10.ViewModels.ControlViewModels;

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

        private void FavoritesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var list = sender as ListView;
            var tappedItem = (IFavorite)e.ClickedItem;
            if (list == null)
            {
                return;
            }            

            if(list.SelectionMode == ListViewSelectionMode.Multiple)
            {
                return;
            }
            
            var listContainer = (FrameworkElement)list.ContainerFromItem(e.ClickedItem);
            RelativePanel ownerPanel = listContainer.FindChild<RelativePanel>("ListItemPanel");
            var flyout = FlyoutBase.GetAttachedFlyout(ownerPanel) as MenuFlyout;                                    
            
            flyout.ShowAt(listContainer);
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
