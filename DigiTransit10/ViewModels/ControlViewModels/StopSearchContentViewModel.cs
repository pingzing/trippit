using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services;
using DigiTransit10.VisualStateFramework;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Windows.Devices.Geolocation;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class StopSearchContentViewModel : StateAwareViewModel, ISearchViewModel
    {
        private const int GeocircleRadiusMeters = 750;
        private const int GeocircleNumberOfPoints = 250;

        private readonly IMessenger _messenger;
        private readonly INetworkService _networkService;
        private readonly IGeolocationService _geolocation;

        private CancellationTokenSource _tokenSource = null;
        public CancellationTokenSource TokenSource => _tokenSource;

        public enum StopSearchState { Overview, Details };
        private StopSearchState _currentState;
        
        public SearchSection OwnedBy { get; private set; }    

        private ObservableCollection<StopSearchElementViewModel> _stopsResultList = new ObservableCollection<StopSearchElementViewModel>();
        public ObservableCollection<StopSearchElementViewModel> StopsResultList
        {
            get { return _stopsResultList; }
            set { Set(ref _stopsResultList, value); }
        }

        private ObservableCollection<TransitLineWithoutStops> _linesAtStop = new ObservableCollection<TransitLineWithoutStops>();
        public ObservableCollection<TransitLineWithoutStops> LinesAtStop
        {
            get { return _linesAtStop; }
            set { Set(ref _linesAtStop, value); }
        }

        private ObservableCollection<TransitStopTime> _departuresAtStop = new ObservableCollection<TransitStopTime>();
        public ObservableCollection<TransitStopTime> DeparturesAtStop
        {
            get { return _departuresAtStop; }
            set
            { Set(ref _departuresAtStop, value); }
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

        private bool _isOverviewLoading = false;
        public bool IsOverviewLoading
        {
            get { return _isOverviewLoading; }
            set { Set(ref _isOverviewLoading, value); }
        }

        private bool _isDetailsLoading = false;
        public bool IsDetailsLoading
        {
            get { return _isDetailsLoading; }
            set { Set(ref _isDetailsLoading, value); }
        }

        private StopSearchElementViewModel _selectedStop = null;
        public StopSearchElementViewModel SelectedStop
        {
            get { return _selectedStop; }
            set
            {
                Set(ref _selectedStop, value);
                SendSelectionChangedMessage(value);
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private string _stopsSearchBoxText;
        public string StopsSearchBoxText
        {
            get { return _stopsSearchBoxText; }
            set { Set(ref _stopsSearchBoxText, value); }
        }        
                   
        public RelayCommand LoadedCommand => new RelayCommand(Loaded);
        public RelayCommand UnloadedCommand => new RelayCommand(Unloaded);
        public RelayCommand<string> SearchStopsCommand => new RelayCommand<string>(SearchStopsAsync);
        public RelayCommand<ITransitLine> ViewLineCommand => new RelayCommand<ITransitLine>(ViewLine);

        public StopSearchContentViewModel(IMessenger messenger, INetworkService network, IGeolocationService geolocation, 
            SearchSection ownedBy, string title)
        {
            _messenger = messenger;
            _networkService = network;
            _geolocation = geolocation;
            OwnedBy = ownedBy;
            Title = title;

            _messenger.Register<MessageTypes.ViewStopDetails>(this, SwitchToDetailedView);            
        }

        private async void SwitchToDetailedView(MessageTypes.ViewStopDetails args)
        {
            RaiseStateChanged(StopSearchState.Details);
            SelectedStop = args.StopSelected;

            LinesAtStop.Clear();
            DeparturesAtStop.Clear();

            IsDetailsLoading = true;
            ApiResult<TransitStopDetails> stopDetailsResponse = await _networkService.GetStopDetails(args.StopSelected.BackingStop.GtfsId, DateTime.Now);
            IsDetailsLoading = false;

            if (stopDetailsResponse.IsFailure)
            {
                //todo: show failed UI somehow
                return;
            }
            LinesAtStop = new ObservableCollection<TransitLineWithoutStops>(
                stopDetailsResponse.Result.LinesThroughStop);

            DeparturesAtStop = new ObservableCollection<TransitStopTime>(
                stopDetailsResponse.Result.Stoptimes
                .Where(x => x.RealtimeDepartureDateTime >= DateTime.Now || x.ScheduledDepartureDateTime >= DateTime.Now)
                .ToList());
        }

        private void SwitchToOverview()
        {
            if (_currentState == StopSearchState.Details)
            {
                RaiseStateChanged(StopSearchState.Overview);
                LinesAtStop.Clear();
                DeparturesAtStop.Clear();
            }
        }        

        private async void SearchStopsAsync(string searchText)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                StopsResultList.Clear();
                MapPlaces.Clear();
                return;
            }

            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
            _tokenSource = new CancellationTokenSource();

            IsOverviewLoading = true;
            ApiResult<IEnumerable<TransitStop>> response = await _networkService.GetStopsAsync(searchText, _tokenSource.Token);
            IsOverviewLoading = false;

            if (response.IsFailure)
            {
                // TODO: Show error in list
                StopsResultList.Clear();
                MapPlaces.Clear();
                return;
            }
            if (_tokenSource.Token.IsCancellationRequested)
            {
                // TODO: Show error in list
                StopsResultList.Clear();
                MapPlaces.Clear();
                return;
            }

            // We explicitly enumerate the list into memory here, otherwise Guid.NewGuid() gets called every time we enumerate the list,
            // making it impossible to link a Map POI to a list element.            
            List<TransitStop> stops = response.Result.ToList();
            StopsResultList = new ObservableCollection<StopSearchElementViewModel>(
                stops.Select(x => new StopSearchElementViewModel(x, _messenger)));

            MapPlaces.Clear();
            MapPlaces.AddRange(stops.Select(x => new BasicMapPoi
            {
                Coords = x.Coords,
                Name = x.NameAndCode,
                Id = x.Id
            }));
        }

        internal async Task MoveNearbyCircleToUser()
        {
            if (OwnedBy != SearchSection.Nearby)
            {
                return;
            }

            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
            _tokenSource = new CancellationTokenSource();

            IsOverviewLoading = true;

            GenericResult<Geoposition> result = await _geolocation.GetCurrentLocationAsync();
            if (result.HasResult)
            {
                MapCircles.Clear();
                _messenger.Send(new MessageTypes.CenterMapOnGeoposition(result.Result.Coordinate.Point.Position));
                MapCircles.Add(new ColoredGeocircle(GeoHelper.GetGeocirclePoints(result.Result.Coordinate.Point, GeocircleRadiusMeters, GeocircleNumberOfPoints)));
                await UpdateNearbyPlaces(new Geocircle(result.Result.Coordinate.Point.Position, GeocircleRadiusMeters), _tokenSource.Token);
            }

            IsOverviewLoading = false;            
        }

        internal async Task MoveNearbyCircle(Geopoint point)
        {
            if (OwnedBy != SearchSection.Nearby)
            {
                return;
            }

            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
            _tokenSource = new CancellationTokenSource();

            IsOverviewLoading = true;

            MapCircles.Clear();
            MapCircles.Add(new ColoredGeocircle(GeoHelper.GetGeocirclePoints(point, GeocircleRadiusMeters, GeocircleNumberOfPoints)));
            await UpdateNearbyPlaces(new Geocircle(point.Position, GeocircleRadiusMeters), _tokenSource.Token);

            IsOverviewLoading = false;
        }        
        
        private async Task UpdateNearbyPlaces(Geocircle circle, CancellationToken token)
        {                                   
            ApiResult<IEnumerable<TransitStop>> response = await _networkService.GetStopsByBoundingRadius(
               (float)circle.Center.Latitude,
               (float)circle.Center.Longitude,
               (int)circle.Radius,
               token
            );            

            if (response.IsFailure)
            {
                // TODO: Show error in list
                StopsResultList.Clear();
                MapPlaces.Clear();
                return;
            }
            if (token.IsCancellationRequested)
            {
                // TODO: show error in list
                StopsResultList.Clear();
                MapPlaces.Clear();
                return;
            }

            // We explicitly enumerate the list into memory here, otherwise Guid.NewGuid() gets called every time we enumerate the list,
            // making it impossible to link a Map POI to a list element.
            List<TransitStop> stops = response.Result.ToList();
            StopsResultList = new ObservableCollection<StopSearchElementViewModel>(
                stops.Select(x => new StopSearchElementViewModel(x, _messenger)));

            MapPlaces.Clear();
            MapPlaces.AddRange(stops.Select(x => new BasicMapPoi
            {
                Coords = x.Coords,
                Name = x.NameAndCode,
                Id = x.Id
            }));
        }

        public void SetMapSelectedPlace(IEnumerable<Guid> obj)
        {
            Guid? nullableId = obj?.FirstOrDefault();
            if (nullableId != null)
            {
                Guid clickedId = nullableId.Value;
                StopSearchElementViewModel matchingStop = StopsResultList.FirstOrDefault(x => x.BackingStop.Id == clickedId);
                if (matchingStop != null)
                {                    
                    if (_currentState == StopSearchState.Details)
                    {
                        SwitchToDetailedView(new MessageTypes.ViewStopDetails(matchingStop));
                    }
                    else
                    {
                        SelectedStop = matchingStop;
                    }
                }
            }
        }

        private void SendSelectionChangedMessage(StopSearchElementViewModel value)
        {
            if (OwnedBy == SearchSection.Nearby)
            {
                _messenger.Send(new MessageTypes.NearbyListSelectionChanged(value.BackingStop));
            }
            if (OwnedBy == SearchSection.Stops)
            {
                _messenger.Send(new MessageTypes.StopsListSelectionChanged(value.BackingStop));
            }
        }

        private void ViewLine(ITransitLine line)
        {
            RaiseStateChanged(StopSearchState.Overview);
            _messenger.Send(new MessageTypes.LineSearchRequested(line));
        }

        private void Loaded()
        {
            BootStrapper.BackRequested += BootStrapper_BackRequested;
        }

        private void Unloaded()
        {
            BootStrapper.BackRequested -= BootStrapper_BackRequested;
        }

        private void BootStrapper_BackRequested(object sender, HandledEventArgs e)
        {
            if (_currentState == StopSearchState.Details)
            {
                e.Handled = true;
                SwitchToOverview();
            }
        }

        public override event VmStateChangeHandler VmStateChangeRequested;
        private void RaiseStateChanged(StopSearchState newState)
        {
            _currentState = newState;
            VmStateChangeRequested?.Invoke(this, new VmStateChangedEventArgs(newState.ToString()));
            _messenger.Send(new MessageTypes.ViewStateChanged(this, _currentState));
        }        
    }
}
