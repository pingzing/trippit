using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Devices.Geolocation;

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

        private CancellationTokenSource _cts;
        private SearchSection _activeSection;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private ObservableCollection<ApiStop> _nearbyStopsResultList = new ObservableCollection<ApiStop>();
        public ObservableCollection<ApiStop> NearbyStopsResultList
        {
            get { return _nearbyStopsResultList; }
            set { Set(ref _nearbyStopsResultList, value); }
        }

        private ObservableCollection<ApiRoute> _linesResultList = new ObservableCollection<ApiRoute>();
        public ObservableCollection<ApiRoute> LinesResultList
        {
            get { return _linesResultList; }
            set { Set(ref _linesResultList, value); }
        }

        private ObservableCollection<ApiStop> _stopsResultList = new ObservableCollection<ApiStop>();
        public ObservableCollection<ApiStop> StopsResultList
        {
            get { return _stopsResultList; }
            set { Set(ref _stopsResultList, value); }
        }

        private ObservableCollection<IMapPoi> _mapPlaces = new ObservableCollection<IMapPoi>();
        public ObservableCollection<IMapPoi> MapPlaces
        {
            get { return _mapPlaces; }
            set { Set(ref _mapPlaces, value); }
        }

        private string _linesSearchBoxText;
        public string LinesSearchBoxText
        {
            get { return _linesSearchBoxText; }
            set { Set(ref _linesSearchBoxText, value); }
        }

        private string _stopsSearchBoxText;
        public string StopsSearchBoxText
        {
            get { return _stopsSearchBoxText; }
            set { Set(ref _stopsSearchBoxText, value); }
        }

        public RelayCommand<GeoboundingBox> UpdateNearbyPlacesCommand => new RelayCommand<GeoboundingBox>(UpdateNearbyPlaces);       
        public RelayCommand<string> SearchLinesCommand => new RelayCommand<string>(SearchLines);
        public RelayCommand<string> SearchStopsCommand => new RelayCommand<string>(SearchStops);
        public RelayCommand<SearchSection> SectionChangedCommand => new RelayCommand<SearchSection>(SectionChanged);        

        public SearchViewModel(INetworkService networkService)
        {
            _networkService = networkService;        
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            _cts?.Cancel();
            return Task.CompletedTask;
        }

        private async void UpdateNearbyPlaces(GeoboundingBox obj)
        {
            if(_activeSection != SearchSection.Nearby)
            {
                return;
            }

            if(_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts = new CancellationTokenSource();

            IsLoading = true;

            var response = await _networkService.GetStopsByBoundingBox(obj, _cts.Token);
                        
            if (response.IsFailure)
            {                
                return;
            }
            NearbyStopsResultList = new ObservableCollection<ApiStop>(response.Result);

            IsLoading = false;
        }

        private async void SearchStops(string searchText)
        {
            if(String.IsNullOrWhiteSpace(searchText))
            {
                StopsResultList.Clear();
                return;
            }

            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            _cts = new CancellationTokenSource();

            IsLoading = true;

            //not sending the token into the network request, because they get called super frequently, and cancelling a network request is a HUGE perf hit on phones
            var response = await _networkService.GetStopsAsync(searchText);
            if(response.IsFailure || _cts.IsCancellationRequested)
            {
                IsLoading = false;
                StopsResultList.Clear();
                return;
            }
            StopsResultList = new ObservableCollection<ApiStop>(response.Result);
            MapPlaces = new ObservableCollection<IMapPoi>(response.Result
                .Select(x => new Place
                    {
                        Lat = x.Lat,
                        Lon = x.Lon,
                        Name = x.Name,
                        Type = ModelEnums.PlaceType.Stop
                    }));

            IsLoading = false;
        }

        private async void SearchLines(string searchText)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                StopsResultList.Clear();
                return;
            }

            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            _cts = new CancellationTokenSource();

            IsLoading = true;

            //not sending the token into the network request, because they get called super frequently, and cancelling a network request is a HUGE perf hit on phones
            var response = await _networkService.GetLinesAsync(searchText);
            if(response.IsFailure || _cts.IsCancellationRequested)
            {
                IsLoading = false;
                StopsResultList.Clear();
                return;
            }            
            LinesResultList = new ObservableCollection<ApiRoute>(response.Result);

            IsLoading = false;            
        }

        private void SectionChanged(SearchSection newSection)
        {
            _activeSection = newSection;
            _cts.Cancel();
        }
    }
}
