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
using Windows.UI;

namespace DigiTransit10.ViewModels
{
    public class FavoritesViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;
        private readonly IFavoritesService _favoritesService;

        public bool IsFavoritesEmpty
        {
            get
            {
                bool isEmpty = true;
                if (Favorites?.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (var list in Favorites)
                    {
                        if (list?.Count > 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        private ObservableCollection<IMapPoi> _mappableFavoritePlaces = new ObservableCollection<IMapPoi>();
        public ObservableCollection<IMapPoi> MappableFavoritePlaces
        {
            get { return _mappableFavoritePlaces; }
            set { Set(ref _mappableFavoritePlaces, value); }
        }

        private ObservableCollection<ColoredMapLine> _mappableFavoriteRoutes = new ObservableCollection<ColoredMapLine>();
        public ObservableCollection<ColoredMapLine> MappableFavoriteRoutes
        {
            get { return _mappableFavoriteRoutes; }
            set { Set(ref _mappableFavoriteRoutes, value); }
        }
 
        private GroupedFavoriteList _groupedFavoritePlaces = new GroupedFavoriteList(AppResources.Favorites_PlacesGroupHeader);
        public GroupedFavoriteList GroupedFavoritePlaces
        {
            get { return _groupedFavoritePlaces; }
            set
            {
                Set(ref _groupedFavoritePlaces, value);
                RaisePropertyChanged(nameof(IsFavoritesEmpty));
            }
        }

        private GroupedFavoriteList _groupedFavoriteRoutes = new GroupedFavoriteList(AppResources.Favorites_RoutesGroupHeader);
        public GroupedFavoriteList GroupedFavoriteRoutes
        {
            get { return _groupedFavoriteRoutes; }
            set
            {
                Set(ref _groupedFavoriteRoutes, value);
                RaisePropertyChanged(nameof(IsFavoritesEmpty));
            }
        }

        private ObservableCollection<GroupedFavoriteList> _favorites = new ObservableCollection<GroupedFavoriteList>();
        public ObservableCollection<GroupedFavoriteList> Favorites
        {
            get { return _favorites; }
            set
            {
                Set(ref _favorites, value);
                RaisePropertyChanged(nameof(IsFavoritesEmpty));
            }
        }

        public RelayCommand AddNewFavoriteCmmand => new RelayCommand(AddNewFavorite);        

        public RelayCommand<IFavorite> DeleteFavoriteCommand => new RelayCommand<IFavorite>(DeleteFavorite);        

        public FavoritesViewModel(INetworkService networkService, IMessenger messengerService, IFavoritesService favoritesService)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _favoritesService = favoritesService;
            _settingsService = SimpleIoc.Default.GetInstance<SettingsService>();

            Favorites.Add(GroupedFavoritePlaces);
            Favorites.Add(GroupedFavoriteRoutes);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            _favoritesService.FavoritesChanged += FavoritesChanged;

            var favorites = await _favoritesService.GetFavoritesAsync();
            if (GroupedFavoritePlaces.Count == 0)
            {
                foreach (IPlace place in favorites.OfType<IPlace>())
                {
                    AddFavoritePlace((IFavorite)place);
                }                

                foreach (FavoriteRoute route in favorites.OfType<FavoriteRoute>())
                {
                    AddFavoriteRoute(route);
                }                
            }            

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            _favoritesService.FavoritesChanged -= FavoritesChanged;
            await Task.CompletedTask;
        }

        private void DeleteFavorite(IFavorite favorite)
        {
            _favoritesService.RemoveFavorite(favorite);
        }

        private void FavoritesChanged(object sender, FavoritesChangedEventArgs args)
        {
            if(args.AddedFavorites?.Count > 0)
            {
                foreach(var favePlace in args.AddedFavorites.OfType<FavoritePlace>())
                {
                    AddFavoritePlace(favePlace);
                }
                foreach (var faveRoute in args.AddedFavorites.OfType<FavoriteRoute>())
                {
                    AddFavoriteRoute(faveRoute);
                }
            }

            if(args.RemovedFavorites?.Count > 0)
            {
                foreach(var deletedFave in args.RemovedFavorites.OfType<FavoritePlace>())
                {
                    RemoveFavoritePlace(deletedFave);
                }
                foreach (var deletedRoute in args.RemovedFavorites.OfType<FavoriteRoute>())
                {
                    RemoveFavoriteRoute(deletedRoute);
                }
            }
        }

        private async void AddNewFavorite()
        {
            var dialog = new AddOrEditFavoriteDialog();
            await dialog.ShowAsync();
        }

        private void AddFavoritePlace(IFavorite place)
        {
            GroupedFavoritePlaces.AddSorted(place);
            MappableFavoritePlaces.Add(place as IMapPoi);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }

        private void RemoveFavoritePlace(IFavorite deletedFave)
        {
            GroupedFavoritePlaces.Remove(deletedFave);
            MappableFavoritePlaces.Remove(deletedFave as IMapPoi);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }

        private void AddFavoriteRoute(IFavorite route)
        {
            GroupedFavoriteRoutes.Add(route);
            var faveRoute = (FavoriteRoute)route;
            IEnumerable<ColoredMapLinePoint> mapPoints = faveRoute.RouteGeometryStrings
                    .SelectMany(str => GooglePolineDecoder.Decode(str))
                    .Select(coords => new ColoredMapLinePoint(coords, Colors.Blue));
            var mapLine = new ColoredMapLine(mapPoints, faveRoute.FavoriteId);
            
            mapLine.FavoriteId = faveRoute.FavoriteId;            

            MappableFavoriteRoutes.Add(mapLine);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }

        private void RemoveFavoriteRoute(IFavorite route)
        {
            GroupedFavoriteRoutes.Remove(route);

            var faveRoute = (FavoriteRoute)route;
            var toRemove = MappableFavoriteRoutes.FirstOrDefault(x => x.FavoriteId == faveRoute.FavoriteId);
            MappableFavoriteRoutes.Remove(toRemove);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }
    }
}