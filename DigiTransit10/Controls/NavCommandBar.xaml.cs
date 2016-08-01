using GalaSoft.MvvmLight.Command;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.Controls
{
    public sealed partial class NavCommandBar : CommandBar
    {
        private readonly INavigationService _navigationService;

        public RelayCommand HomeCommand => new RelayCommand(GoHome);
        public RelayCommand FavoritesCommand => new RelayCommand(GoFavorites);

        public NavCommandBar()
        {
            this.InitializeComponent();            
            Views.Busy.BusyChanged += BusyView_BusyChanged;

            _navigationService = App.Current.NavigationService;
            _navigationService.Frame.Navigated += Frame_Navigated;
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            HomeButton.Tag = false;
            FavoritesButton.Tag = false;

            if(_navigationService.CurrentPageType == typeof(Views.MainPage))
            {
                HomeButton.Tag = true;
            }
            if(_navigationService.CurrentPageType == typeof(Views.FavoritesPage))
            {
                FavoritesButton.Tag = true;
            }
        }

        private void BusyView_BusyChanged(object sender, bool newIsBusy)
        {
            this.IsEnabled = !newIsBusy;
        }

        private void GoHome()
        {
            _navigationService.NavigateAsync(typeof(Views.MainPage));
        }

        private void GoFavorites()
        {
            _navigationService.NavigateAsync(typeof(Views.FavoritesPage));
        }
    }
}
