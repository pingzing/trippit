using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services;
using DigiTransit10.Services.SettingsServices;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using Template10.Mvvm;
using System;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Collections.Generic;
using DigiTransit10.Controls;
using DigiTransit10.ExtensionMethods;
using DigiTransit10.Localization.Strings;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace DigiTransit10.ViewModels
{
    public class FavoritesViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;
        private readonly IFavoritesService _favoritesService;

        private ObservableCollection<IMapPoi> _mappableFavoritePlaces = new ObservableCollection<IMapPoi>();
        public ObservableCollection<IMapPoi> MappableFavoritePlaces
        {
            get { return _mappableFavoritePlaces; }
            set { Set(ref _mappableFavoritePlaces, value); }
        }

        private GroupedFavoriteList _groupedFavoritePlaces = new GroupedFavoriteList(AppResources.Favorites_PlacesGroupHeader);
        public GroupedFavoriteList GroupedFavoritePlaces
        {
            get { return _groupedFavoritePlaces; }
            set { Set(ref _groupedFavoritePlaces, value); }
        }

        private ObservableCollection<GroupedFavoriteList> _favorites = new ObservableCollection<GroupedFavoriteList>();
        public ObservableCollection<GroupedFavoriteList> Favorites
        {
            get { return _favorites; }
            set { Set(ref _favorites, value); }
        }

        private readonly RelayCommand<IFavorite> _deleteFavoriteCommand = null;
        public RelayCommand<IFavorite> DeleteFavoriteCommand => _deleteFavoriteCommand ?? new RelayCommand<IFavorite>(DeleteFavorite);

        public FavoritesViewModel(INetworkService networkService, IMessenger messengerService, IFavoritesService favoritesService)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _favoritesService = favoritesService;
            _settingsService = SimpleIoc.Default.GetInstance<SettingsService>();
            _messengerService.Register<MessageTypes.FavoritesChangedMessage>(this, FavoritesChanged);            
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            foreach (var place in await _favoritesService.GetFavoritesAsync())
            {
                AddFavoritePlace(place);
            }

            Favorites.Add(GroupedFavoritePlaces);

            await Task.CompletedTask;
        }

        private void DeleteFavorite(IFavorite favorite)
        {
            _favoritesService.RemoveFavorite(favorite);
        }

        private void FavoritesChanged(MessageTypes.FavoritesChangedMessage message)
        {
            if(message.AddedFavorites?.Count > 0)
            {
                foreach(var favePlace in message.AddedFavorites.OfType<FavoritePlace>())
                {
                    AddFavoritePlace(favePlace);
                }
                foreach (var faveRoute in message.AddedFavorites.OfType<FavoriteRoute>())
                {
                    //todo:add to favorite routes list
                }
            }

            if(message.RemovedFavorites?.Count > 0)
            {
                foreach(var deletedFave in message.RemovedFavorites.OfType<FavoritePlace>())
                {
                    RemoveFavoritePlace(deletedFave);
                }
                foreach (var deletedRoute in message.RemovedFavorites.OfType<FavoriteRoute>())
                {
                    //todo:remove from favorite routes
                }
            }
        }

        private void AddFavoritePlace(IFavorite place)
        {
            GroupedFavoritePlaces.AddSorted(place);
            MappableFavoritePlaces.Add(place as IMapPoi);
        }

        private void RemoveFavoritePlace(IFavorite deletedFave)
        {
            GroupedFavoritePlaces.Remove(deletedFave);
            MappableFavoritePlaces.Remove(deletedFave as IMapPoi);
        }
    }
}
