using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
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

namespace DigiTransit10.ViewModels
{
    public enum SearchSection
    {
        Nearby,
        Lines,
        Stops,
        None
    }

    public class SearchViewModel : ViewModelBase
    {
        private const int GeocircleRadius = 750;
        private const int GeocircleNumberOfPoints = 100;
        private readonly INetworkService _networkService;
        private readonly IGeolocationService _geolocation;

        private List<IMapPoi> _hiddenMapPlaces = new List<IMapPoi>();
        private List<IMapPoi> _hiddenMapNearbyPlaces = new List<IMapPoi>();
        private List<ColoredMapLine> _hiddenMapLines = new List<ColoredMapLine>();
        private List<IMapPoi> _hiddenMapLinePlaces = new List<IMapPoi>();
        private List<ColoredGeocircle> _hiddenMapCircles = new List<ColoredGeocircle>();
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

        private ObservableCollection<TransitStop> _nearbyStopsResultList = new ObservableCollection<TransitStop>();
        public ObservableCollection<TransitStop> NearbyStopsResultList
        {
            get { return _nearbyStopsResultList; }
            set { Set(ref _nearbyStopsResultList, value); }
        }

        private ObservableCollection<LineSearchElementViewModel> _linesResultList = new ObservableCollection<LineSearchElementViewModel>();
        public ObservableCollection<LineSearchElementViewModel> LinesResultList
        {
            get { return _linesResultList; }
            set { Set(ref _linesResultList, value); }
        }

        private ObservableCollection<TransitStop> _stopsResultList = new ObservableCollection<TransitStop>();
        public ObservableCollection<TransitStop> StopsResultList
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
        public RelayCommand MoveNearbyCircleToUserCommand => new RelayCommand(MoveNearbyCircleToUser);
        public RelayCommand<string> SearchLinesCommand => new RelayCommand<string>(SearchLines);
        public RelayCommand<string> SearchStopsCommand => new RelayCommand<string>(SearchStops);
        public RelayCommand<SearchSectionChangedEventArgs> SectionChangedCommand => new RelayCommand<SearchSectionChangedEventArgs>(SectionChanged);
        public RelayCommand<LineSearchElementViewModel> UpdateSelectedLineCommand => new RelayCommand<LineSearchElementViewModel>(UpdateSelectedLine,
            UpdateSelectedLineCanExecute);
        private bool UpdateSelectedLineCanExecute(LineSearchElementViewModel arg)
        {
            return arg != null;
        }

        public SearchViewModel(INetworkService networkService, IGeolocationService geolocation)
        {
            _networkService = networkService;
            _geolocation = geolocation;
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (_activeSection == SearchSection.Nearby)
            {
                MoveNearbyCircleToUser();
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            MapCircles.Clear();
            MapPlaces.Clear();
            MapLines.Clear();
            _hiddenMapPlaces.Clear();
            _hiddenMapNearbyPlaces.Clear();
            _hiddenMapLines.Clear();
            _hiddenMapLinePlaces.Clear();
            _hiddenMapCircles.Clear();
            _cts?.Cancel();
            return Task.CompletedTask;
        }

        private async void MoveNearbyCircle(Geopoint point)
        {
            MapCircles.Clear();
            MapCircles.Add(new ColoredGeocircle(GeoHelper.GetGeocirclePoints(point, GeocircleRadius, GeocircleNumberOfPoints)));
            await UpdateNearbyPlaces(new Geocircle(point.Position, GeocircleRadius));
        }

        private async void MoveNearbyCircleToUser()
        {
            IsNearbyStopsLoading = true;
            GenericResult<Geoposition> result = await _geolocation.GetCurrentLocationAsync();
            if (result.HasResult)
            {
                MapCircles.Clear();
                Messenger.Default.Send(new MessageTypes.CenterMapOnGeoposition(result.Result.Coordinate.Point.Position));
                MapCircles.Add(new ColoredGeocircle(GeoHelper.GetGeocirclePoints(result.Result.Coordinate.Point, GeocircleRadius, GeocircleNumberOfPoints)));
                await UpdateNearbyPlaces(new Geocircle(result.Result.Coordinate.Point.Position, GeocircleRadius));
            }
            IsNearbyStopsLoading = false;
        }

        private async Task UpdateNearbyPlaces(Geocircle circle)
        {
            if (_activeSection != SearchSection.Nearby)
            {
                return;
            }

            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts = new CancellationTokenSource();
            IsNearbyStopsLoading = true;
            ApiResult<IEnumerable<TransitStop>> response = await _networkService.GetStopsByBoundingRadius(
                (float)circle.Center.Latitude,
                (float)circle.Center.Longitude,
                (int)circle.Radius,
                _cts.Token
            );
            if (response.IsFailure || _cts.IsCancellationRequested)
            {
                return;
            }
            NearbyStopsResultList = new ObservableCollection<TransitStop>(response.Result);
            IsNearbyStopsLoading = false;
            MapPlaces = new ObservableCollection<IMapPoi>(response.Result
                .Select(x => new BasicMapPoi
                {
                    Coords = x.Coords,
                    Name = x.NameAndCode
                })
                .ToList());
        }

        private async void SearchStops(string searchText)
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

            IsStopsLoading = true;

            var response = await _networkService.GetStopsAsync(searchText, _cts.Token);
            if (response.IsFailure || _cts.IsCancellationRequested)
            {
                IsStopsLoading = false;
                StopsResultList.Clear();
                return;
            }
            StopsResultList = new ObservableCollection<TransitStop>(response.Result.Select(x => new TransitStop
            {
                Name = x.Name,
                Code = x.Code,
                Coords = BasicGeopositionExtensions.Create(0.0, x.Lon, x.Lat)
            }));
            MapPlaces = new ObservableCollection<IMapPoi>(StopsResultList
                .Select(x => new Place
                {
                    Lat = (float)x.Coords.Latitude,
                    Lon = (float)x.Coords.Longitude,
                    Name = x.NameAndCode,
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
            if (response.IsFailure || _cts.IsCancellationRequested)
            {
                IsLinesLoading = false;
                StopsResultList.Clear();
                return;
            }
            LinesResultList = new ObservableCollection<LineSearchElementViewModel>(response.Result.Select(x => new LineSearchElementViewModel { BackingLine = x }));

            IsLinesLoading = false;
        }

        private void UpdateSelectedLine(LineSearchElementViewModel element)
        {
            MapLines.Clear();

            List<ColoredMapLinePoint> linePoints = element
                .BackingLine
                .Points
                .Select(x => new ColoredMapLinePoint(
                                    BasicGeopositionExtensions.Create(0.0, x.Longitude, x.Latitude),
                                    HslColors.GetModeColor(element.BackingLine.TransitMode)))
                .ToList();
            var mapLine = new ColoredMapLine(linePoints);
            MapLines = new ObservableCollection<ColoredMapLine>(new List<ColoredMapLine> { mapLine });
            List<IMapPoi> stops = new List<IMapPoi>();
            foreach(var stop in element.BackingLine.Stops)
            {
                stops.Add(new BasicMapPoi {Coords = stop.Coords, Name = stop.Name });
            }
            MapPlaces = new ObservableCollection<IMapPoi>(stops);
        }

        //todo: handle narrow <-> wide changes a little better. since the pivot selection isn't synced, the circles/icons/etc on the map get a little out of sync
        private void SectionChanged(SearchSectionChangedEventArgs args)
        {
            _activeSection = args.NewSection;
            switch (args.OldSection)
            {
                case SearchSection.Lines:
                    _hiddenMapLines.Clear();
                    _hiddenMapLines.AddRange(MapLines);
                    _hiddenMapLinePlaces.Clear();
                    _hiddenMapLinePlaces.AddRange(MapPlaces);
                    MapPlaces.Clear();
                    MapLines.Clear();
                    break;
                case SearchSection.Nearby:
                    _hiddenMapNearbyPlaces.Clear();
                    _hiddenMapNearbyPlaces.AddRange(MapPlaces);
                    _hiddenMapCircles.Clear();
                    _hiddenMapCircles.AddRange(MapCircles);
                    MapPlaces.Clear();
                    MapCircles.Clear();
                    break;
                case SearchSection.Stops:
                    _hiddenMapPlaces.Clear();
                    _hiddenMapPlaces.AddRange(MapPlaces);
                    MapPlaces.Clear();
                    break;
                case SearchSection.None:
                    break; //do nothing
                default:
                    throw new ArgumentOutOfRangeException("newSection", "You forgot to add the last case in SectionChanged on the SearchViewModel, dunce!");
            }


            switch (_activeSection)
            {
                case SearchSection.Lines:
                    MapLines.AddRange(_hiddenMapLines);
                    MapPlaces.AddRange(_hiddenMapLinePlaces);
                    break;
                case SearchSection.Nearby:
                    MapCircles.AddRange(_hiddenMapCircles);
                    MapPlaces.AddRange(_hiddenMapNearbyPlaces);
                    break;
                case SearchSection.Stops:
                    MapPlaces.AddRange(_hiddenMapPlaces);
                    break;
                case SearchSection.None:
                    throw new ArgumentOutOfRangeException("newSection", "You can't have the ActiveSection be None, dunce!");
                default:
                    throw new ArgumentOutOfRangeException("newSection", "You forgot to add the last case in SectionChanged on the SearchViewModel, dunce!");
            }
        }
    }
}
