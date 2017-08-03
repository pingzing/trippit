using System.Diagnostics;
using Trippit.Controls;
using Trippit.Models;
using Trippit.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Trippit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritesPage : AnimatedPage
    {
        public FavoritesViewModel ViewModel { get; set; }

        public FavoritesPage()
        {
            this.InitializeComponent();
            ViewModel = DataContext as FavoritesViewModel;            

            //Doing this in code-behind, because doing it in XAML breaks the XAML designer.
            var collectionViewSourceBinding = new Binding();
            collectionViewSourceBinding.Source = ViewModel.Favorites;
            collectionViewSourceBinding.Mode = BindingMode.OneWay;

            BindingOperations.SetBinding(FavoritesViewSource,
                CollectionViewSource.SourceProperty,
                collectionViewSourceBinding);

            this.Unloaded += FavoritesPage_Unloaded;
        }

        private void FavoritesPage_Unloaded(object sender, RoutedEventArgs e)
        {            
            this.Bindings.StopTracking();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("Favorites navigated to!");
            base.OnNavigatedTo(e);
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

            var tappedPlace = tappedItem as FavoritePlace;
            if (tappedPlace != null)
            {
                if (ViewModel.SetAsDestinationCommand.CanExecute(tappedItem))
                {
                    ViewModel.SetAsDestinationCommand.Execute(tappedItem);
                }
            }
            var tappedRoute = tappedItem as FavoriteRoute;
            if(tappedRoute != null)
            {
                if(ViewModel.SetAsRouteCommand.CanExecute(tappedRoute))
                {
                    ViewModel.SetAsRouteCommand.Execute(tappedRoute);
                }
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var boundingBox = this.FavoritesMap.GetAllMapElementsBoundingBox();
            if (boundingBox != null)
            {
                await this.FavoritesMap.TrySetViewBoundsAsync(boundingBox, new Thickness(450, 50, 50, 50), MapAnimationKind.None);
            }
        }

        private void PinCommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            MovableAppBarButton button = (MovableAppBarButton)sender;
            FlyoutBase.ShowAttachedFlyout(button);
        }
    }
}
