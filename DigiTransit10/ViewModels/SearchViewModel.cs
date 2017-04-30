using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Services;
using DigiTransit10.Styles;
using DigiTransit10.ViewModels.ControlViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Navigation;
using static DigiTransit10.ViewModels.ControlViewModels.StopSearchContentViewModel;

namespace DigiTransit10.ViewModels
{
    public enum SearchSection
    {
        Nearby,
        Lines,
        Stops        
    }

    public class SearchViewModel : ViewModelBase
    {        
        private readonly INetworkService _networkService;
        private readonly IGeolocationService _geolocation;
        private readonly IMessenger _messenger;

        private StopSearchContentViewModel _nearbyStopsViewModel;
        private StopSearchContentViewModel _searchStopsViewModel;
        private LineSearchContentViewModel _linesSearchViewModel;

        private ObservableCollection<ISearchViewModel> _searchViewModels = new ObservableCollection<ISearchViewModel>();
        public ObservableCollection<ISearchViewModel> SearchViewModels
        {
            get { return _searchViewModels; }
            set { Set(ref _searchViewModels, value); }
        }        

        private ObservableCollection<IMapPoi> _mapPlaces = new ObservableCollection<IMapPoi>();
        public ObservableCollection<IMapPoi> MapPlaces
        {
            get { return _mapPlaces; }
            set { Set(ref _mapPlaces, value); }
        }

        private ObservableCollection<ColoredMapLine> _mapLines = new ObservableCollection<ColoredMapLine>();
        public ObservableCollection<ColoredMapLine> MapLines
        {
            get { return _mapLines; }
            set { Set(ref _mapLines, value); }
        }

        private ObservableCollection<ColoredGeocircle> _mapCircles = new ObservableCollection<ColoredGeocircle>();
        public ObservableCollection<ColoredGeocircle> MapCircles
        {
            get { return _mapCircles; }
            set { Set(ref _mapCircles, value); }
        }       

        private bool _childIsInDetailedState = false;
        public bool ChildIsInDetailedState
        {
            get { return _childIsInDetailedState; }
            set { Set(ref _childIsInDetailedState, value); }
        }

        private ISearchViewModel _selectedPivot;
        public ISearchViewModel SelectedPivot
        {
            get { return _selectedPivot; }
            set
            {
                Set(ref _selectedPivot, value);
                UpdateSelectedPivot(value);
            }
        }

        public RelayCommand<Geopoint> MoveNearbyCircleCommand => new RelayCommand<Geopoint>(MoveNearbyCircle);
        public RelayCommand MoveNearbyCircleToUserCommand => new RelayCommand(MoveNearbyCircleToUser);
        public RelayCommand<IEnumerable<Guid>> MapElementTappedCommand => new RelayCommand<IEnumerable<Guid>>(MapElementTapped);

        public SearchViewModel(INetworkService networkService, IGeolocationService geolocation, IMessenger messenger)
        {
            _networkService = networkService;
            _geolocation = geolocation;
            _messenger = messenger;

            _nearbyStopsViewModel = new StopSearchContentViewModel(_messenger, _networkService, 
                _geolocation, SearchSection.Nearby, AppResources.SearchPage_NearbyHeader);
            _linesSearchViewModel = new LineSearchContentViewModel(_networkService, _messenger, 
                SearchSection.Lines, AppResources.SearchPage_LinesHeader);
            _searchStopsViewModel = new StopSearchContentViewModel(_messenger, _networkService, 
                _geolocation, SearchSection.Stops, AppResources.SearchPage_StopsHeader);
            _messenger.Register<MessageTypes.ViewStateChanged>(this, ChildStateChanged);            

            SearchViewModels.Add(_nearbyStopsViewModel);
            SearchViewModels.Add(_linesSearchViewModel);
            SearchViewModels.Add(_searchStopsViewModel);

            SelectedPivot = _nearbyStopsViewModel;
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SelectedPivot.OwnedBy == SearchSection.Nearby)
            {
                MoveNearbyCircleToUser();
            }

            MapCircles = SelectedPivot.MapCircles;
            MapPlaces = SelectedPivot.MapPlaces;
            MapLines = SelectedPivot.MapLines;

            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            MapCircles = null;
            MapPlaces = null;
            MapLines = null;

            //TODO: HACK! For some reason, if we leave while SelectedPivot is anything by
            // the first pivot in the collection, when we come back to the page, we hard crash with
            // no stack trace and no (useful) error message.
            // Theory: Maybe related to the memory leak we keep seeing with the MapControl?
            // Further theory: Probably not memory leak related, more like some race condition deep in the binding system.
            // It RARELY happens if navigating to the page and quickly switching pivots at a very specific instant.
            SelectedPivot = _nearbyStopsViewModel;
            //-----end hack
            
            return Task.CompletedTask;
        }

        private async void MoveNearbyCircle(Geopoint point)
        {
            SelectedPivot = _nearbyStopsViewModel;
            await _nearbyStopsViewModel.MoveNearbyCircle(point);
        }

        private async void MoveNearbyCircleToUser()
        {
            SelectedPivot = _nearbyStopsViewModel;
            await _nearbyStopsViewModel.MoveNearbyCircleToUser();
        }                
        
        private void UpdateSelectedPivot(ISearchViewModel newPivotSelection)
        {                                   
            MapCircles = newPivotSelection.MapCircles;
            MapPlaces = newPivotSelection.MapPlaces;
            MapLines = newPivotSelection.MapLines;
            return;            
        }

        private void ChildStateChanged(MessageTypes.ViewStateChanged args)
        {
            if (ReferenceEquals(args.Sender, _nearbyStopsViewModel) && SelectedPivot.OwnedBy == SearchSection.Nearby
                || ReferenceEquals(args.Sender, _searchStopsViewModel) && SelectedPivot.OwnedBy == SearchSection.Stops)
            {
                ChildIsInDetailedState = args.ViewState == StopSearchState.Details;
            }            
        }

        private void MapElementTapped(IEnumerable<Guid> tappedIds)
        {
            SelectedPivot.SetMapSelectedPlace(tappedIds);
        }
    }
}
