using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Services;
using DigiTransit10.Styles;
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
using DigiTransit10.ExtensionMethods;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;

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

        private CancellationTokenSource _cts;
        private SearchSection _activeSection;

        public bool IsLoading => IsNearbyStopsLoading
            || IsLinesLoading
            || IsStopsLoading;

        private bool _isNearbyStopsLoading;
        public bool IsNearbyStopsLoading
        {
            get { return _isNearbyStopsLoading; }
            set
            {
                _isNearbyStopsLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
        }

        private bool _isLinesLoading;
        public bool IsLinesLoading
        {
            get { return _isLinesLoading; }
            set
            {
                _isLinesLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
        }

        private bool _isStopsLoading;
        public bool IsStopsLoading
        {
            get { return _isStopsLoading; }
            set
            {
                _isStopsLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
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

        public RelayCommand<Geopoint> MoveNearbyCircleCommand => new RelayCommand<Geopoint>(MoveNearbyCircle);
        public RelayCommand<string> SearchLinesCommand => new RelayCommand<string>(SearchLines);
        public RelayCommand<string> SearchStopsCommand => new RelayCommand<string>(SearchStops);
        public RelayCommand<SearchSection> SectionChangedCommand => new RelayCommand<SearchSection>(SectionChanged);
        public RelayCommand<ApiRoute> UpdateSelectedLineCommand => new RelayCommand<ApiRoute>(UpdateSelectedLine,
            UpdateSelectedLineCanExecute);
        private bool UpdateSelectedLineCanExecute(ApiRoute arg)
        {
            return arg != null;
        }

        public SearchViewModel(INetworkService networkService, IGeolocationService geolocation)
        {
            _networkService = networkService;
            _geolocation = geolocation;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            MapCircles.Clear();
            GenericResult<Geoposition> result = await _geolocation.GetCurrentLocationAsync();
            if(result.HasResult)
            {
                Messenger.Default.Send(new MessageTypes.CenterMapOnGeoposition(result.Result.Coordinate.Point.Position));
                MapCircles.Add(new ColoredGeocircle(GeoHelper.GetGeocirclePoints(result.Result.Coordinate.Point, 750, 100)));
                await UpdateNearbyPlaces(new Geocircle(result.Result.Coordinate.Point.Position, 750));
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            _cts?.Cancel();
            return Task.CompletedTask;
        }

        private async void MoveNearbyCircle(Geopoint point)
        {
            MapCircles.Clear();
            MapCircles.Add(new ColoredGeocircle(GeoHelper.GetGeocirclePoints(point, 750, 100)));
            await UpdateNearbyPlaces(new Geocircle(point.Position, 750));
        }

        private async Task UpdateNearbyPlaces(Geocircle circle)
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
            IsNearbyStopsLoading = true;
            ApiResult<IEnumerable<ApiStop>> response = await _networkService.GetStopsByBoundingRadius((float)circle.Center.Latitude, (float)circle.Center.Longitude, (int)circle.Radius, _cts.Token);
            if (response.IsFailure || _cts.IsCancellationRequested)
            {
                return;
            }
            NearbyStopsResultList = new ObservableCollection<ApiStop>(response.Result);
            IsNearbyStopsLoading = false;
            MapPlaces = new ObservableCollection<IMapPoi>(response.Result
                .Select(x => new BasicMapPoi
                    {
                        Coords = BasicGeopositionExtensions.Create(0, x.Lon, x.Lat),
                        Name = x.Name
                    })
                .ToList());
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

            IsStopsLoading = true;

            var response = await _networkService.GetStopsAsync(searchText, _cts.Token);
            if(response.IsFailure || _cts.IsCancellationRequested)
            {
                IsStopsLoading = false;
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

            IsStopsLoading = false;
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

            IsLinesLoading = true;

            var response = await _networkService.GetLinesAsync(searchText, _cts.Token);
            if(response.IsFailure || _cts.IsCancellationRequested)
            {
                IsLinesLoading = false;
                StopsResultList.Clear();
                return;
            }
            LinesResultList = new ObservableCollection<ApiRoute>(response.Result);            

            IsLinesLoading = false;
        }

        private void UpdateSelectedLine(ApiRoute obj)
        {
            MapLines.Clear();

            List<ColoredMapLinePoint> linePoints = obj.Patterns
                .First()
                .Geometry                
                .Select(x => new ColoredMapLinePoint(BasicGeopositionExtensions.Create(0.0, x.Lon, x.Lat), HslColors.GetModeColor(obj.Mode)))
                .ToList();
            var mapLine = new ColoredMapLine(linePoints);
            MapLines = new ObservableCollection<ColoredMapLine>(new List<ColoredMapLine> { mapLine });
        }

        private void SectionChanged(SearchSection newSection)
        {
            _activeSection = newSection;            
        }
    }
}
