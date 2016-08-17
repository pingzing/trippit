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

namespace DigiTransit10.ViewModels
{
    public class FavoritesViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;

        private GroupedFavoriteList _favoritePlaces = new GroupedFavoriteList(AppResources.Favorites_PlacesGroupHeader);
        public GroupedFavoriteList FavoritePlaces
        {
            get { return _favoritePlaces; }
            set { Set(ref _favoritePlaces, value); }
        }

        private ObservableCollection<GroupedFavoriteList> _favorites = new ObservableCollection<GroupedFavoriteList>();
        public ObservableCollection<GroupedFavoriteList> Favorites
        {
            get { return _favorites; }
            set { Set(ref _favorites, value); }
        }

        private readonly RelayCommand<IFavorite> _deleteFavoriteCommand = null;
        public RelayCommand<IFavorite> DeleteFavoriteCommand => _deleteFavoriteCommand ?? new RelayCommand<IFavorite>(DeleteFavorite);

        public FavoritesViewModel(INetworkService networkService, IMessenger messengerService)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = SimpleIoc.Default.GetInstance<SettingsService>();
            _messengerService.Register<MessageTypes.FavoritesChangedMessage>(this, FavoritesChanged);

            foreach (var place in _settingsService.Favorites)
            {
                FavoritePlaces.AddSorted(place);
            }

            Favorites.Add(FavoritePlaces);
        }

        private void DeleteFavorite(IFavorite favorite)
        {
            _settingsService.RemoveFavorite(favorite);
        }

        private void FavoritesChanged(MessageTypes.FavoritesChangedMessage message)
        {
            if(message.AddedFavorites?.Count > 0)
            {
                foreach(var favePlace in message.AddedFavorites.OfType<FavoritePlace>())
                {
                    FavoritePlaces.AddSorted(favePlace);
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
                    FavoritePlaces.Remove(deletedFave);
                }
                foreach (var deletedRoute in message.RemovedFavorites.OfType<FavoriteRoute>())
                {
                    //todo:remove from favorite routes
                }
            }
        }
    }
}
