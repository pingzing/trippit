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
        private readonly IMessenger _messenger;
        private readonly INetworkService _networkService;

        private CancellationTokenSource _cts = null;

        public enum StopSearchState { Overview, Details };
        private StopSearchState _currentState;
        
        public SearchSection OwnedBy { get; private set; }

        public RelayCommand LoadedCommand => new RelayCommand(Loaded);
        public RelayCommand UnloadedCommand => new RelayCommand(Unloaded);
        public RelayCommand<string> SearchStopsCommand => new RelayCommand<string>(SearchStopsAsync);

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
            set { Set(ref _departuresAtStop, value); }
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

        public StopSearchContentViewModel(IMessenger messenger, INetworkService network, SearchSection ownedBy, string title)
        {
            _messenger = messenger;
            _networkService = network;
            OwnedBy = ownedBy;
            Title = title;

            _messenger.Register<MessageTypes.ViewStopDetails>(this, SwitchToDetailedView);
        }

        private async void SwitchToDetailedView(MessageTypes.ViewStopDetails args)
        {
            RaiseStateChanged(StopSearchState.Details);
            SelectedStop = args.StopSelected;
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

            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            _cts = new CancellationTokenSource();

            IsOverviewLoading = true;
            ApiResult<IEnumerable<TransitStop>> response = await _networkService.GetStopsAsync(searchText, _cts.Token);
            IsOverviewLoading = false;

            if (response.IsFailure)
            {
                // TODO: Show error in list
                StopsResultList.Clear();
                MapPlaces.Clear();
                return;
            }
            if (_cts.Token.IsCancellationRequested)
            {
                // TODO: Show error in list
                StopsResultList.Clear();
                MapPlaces.Clear();
                return;
            }

            StopsResultList = new ObservableCollection<StopSearchElementViewModel>(
                response.Result.Select(x => new StopSearchElementViewModel(x, _messenger)));

            MapPlaces.Clear();
            MapPlaces.AddRange(response.Result.Select(x => new BasicMapPoi
            {
                Coords = x.Coords,
                Name = x.NameAndCode,
            }));
        }

        // This gets special handling because the source of this request ultimately 
        // has to come from the parent, not this control.
        public async Task UpdateNearbyPlacesAsync(Geocircle circle, CancellationToken token)
        {
            // Cancel any outstanding tokens, but use the one we get passed from the parent SearchPage ViewModel.
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            IsOverviewLoading = true;
            ApiResult<IEnumerable<TransitStop>> response = await _networkService.GetStopsByBoundingRadius(
               (float)circle.Center.Latitude,
               (float)circle.Center.Longitude,
               (int)circle.Radius,
               token
            );
            IsOverviewLoading = false;

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

            StopsResultList = new ObservableCollection<StopSearchElementViewModel>(
                response.Result.Select(x => new StopSearchElementViewModel(x, _messenger)));

            MapPlaces.Clear();
            MapPlaces.AddRange(response.Result.Select(x => new BasicMapPoi
            {
                Coords = x.Coords,
                Name = x.NameAndCode,
            }));
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
