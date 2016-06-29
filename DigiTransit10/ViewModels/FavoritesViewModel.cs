using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using DigiTransit10.Services;
using DigiTransit10.Services.SettingsServices;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels
{
    public class FavoritesViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly SettingsService _settingsService;

        private ObservableCollection<IFavorite> _favorites = new ObservableCollection<IFavorite>();
        public ObservableCollection<IFavorite> Favorites
        {
            get { return _favorites; }
            set { Set(ref _favorites, value); }
        }

        public FavoritesViewModel(INetworkService networkService)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Favorites.Add(new FavoriteRoute { UserChosenName = "Home -> Work" });
                Favorites.Add(new FavoritePlace { UserChosenName = "Helsinki" });
            }

            _networkService = networkService;
            _settingsService = SimpleIoc.Default.GetInstance<SettingsService>();
            foreach(var place in _settingsService.FavoritePlaces)
            {
                Favorites.AddSorted(place);
            }
            foreach(var route in _settingsService.FavoriteRoutes)
            {
                Favorites.AddSorted(route);
            }
        }
    }
}
