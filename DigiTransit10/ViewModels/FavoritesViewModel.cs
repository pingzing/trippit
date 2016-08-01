using DigiTransit10.ExtensionMethods;
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

namespace DigiTransit10.ViewModels
{
    public class FavoritesViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;

        private ObservableCollection<IFavorite> _favorites = new ObservableCollection<IFavorite>();
        public ObservableCollection<IFavorite> Favorites
        {
            get { return _favorites; }
            set
            {
                Set(ref _favorites, value);
                RaisePropertyChanged(nameof(FavoritePlaces));
            }
        }

        public List<IPlace> FavoritePlaces => Favorites.Where(x => x is IPlace).Select(x => x as IPlace).ToList();

        private RelayCommand<IFavorite> _deleteFavoriteCommand = null;
        public RelayCommand<IFavorite> DeleteFavoriteCommand => _deleteFavoriteCommand ?? new RelayCommand<IFavorite>(DeleteFavorite);

        public FavoritesViewModel(INetworkService networkService, IMessenger messengerService)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Favorites.Add(new FavoriteRoute { UserChosenName = "Home -> Work" });
                Favorites.Add(new FavoritePlace { UserChosenName = "Helsinki" });
            }

            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = SimpleIoc.Default.GetInstance<SettingsService>();
            foreach(var place in _settingsService.Favorites)
            {
                Favorites.AddSorted(place);
            }

            _messengerService.Register<MessageTypes.FavoritesChangedMessage>(this, FavoritesChanged);
            Favorites.CollectionChanged += Favorites_CollectionChanged;
        }

        private void Favorites_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(FavoritePlaces));
        }

        private void DeleteFavorite(IFavorite favorite)
        {
            _settingsService.RemoveFavorite(favorite);
        }

        private void FavoritesChanged(MessageTypes.FavoritesChangedMessage message)
        {
            if(message.AddedFavorites?.Count > 0)
            {
                foreach(var newFave in message.AddedFavorites)
                {
                    Favorites.AddSorted(newFave);
                }
            }

            if(message.RemovedFavorites?.Count > 0)
            {
                foreach(var deletedFave in message.RemovedFavorites)
                {
                    Favorites.Remove(deletedFave);
                }
            }
        }
    }
}
