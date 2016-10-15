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
    public class SearchViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;

        private CancellationTokenSource _cts;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private ObservableCollection<object> _nearbyPlacesResultList = new ObservableCollection<object>();
        public ObservableCollection<object> NearbyPlacesResultList
        {
            get { return _nearbyPlacesResultList; }
            set { Set(ref _nearbyPlacesResultList, value); }
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
            NearbyPlacesResultList = new ObservableCollection<object>(response.Result);

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
    }
}
