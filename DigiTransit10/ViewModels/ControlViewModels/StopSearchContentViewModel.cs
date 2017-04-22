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
    public class StopSearchContentViewModel : StateAwareViewModel
    {
        private readonly IMessenger _messenger;
        private readonly INetworkService _networkService;

        public enum StopSearchState { Overview, Details };
        private StopSearchState _currentState;

        public enum OwnerSearchPivot { NearbyStops, Stops };
        public OwnerSearchPivot OwnedBy { get; private set; }

        public RelayCommand LoadedCommand => new RelayCommand(Loaded);
        public RelayCommand UnloadedCommand => new RelayCommand(Unloaded);

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

        private bool _isDetailsLoading = false;
        public bool IsDetailsLoading
        {
            get { return _isDetailsLoading; }
            set { Set(ref _isDetailsLoading, value); }
        }

        private TransitStop _selectedStop = null;
        public TransitStop SelectedStop
        {
            get { return _selectedStop; }
            set { Set(ref _selectedStop, value); }
        }

        private string _stopsSearchBoxText;
        public string StopsSearchBoxText
        {
            get { return _stopsSearchBoxText; }
            set { Set(ref _stopsSearchBoxText, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public StopSearchContentViewModel(IMessenger messenger, INetworkService network, OwnerSearchPivot ownedBy, string title)
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
            ApiResult<TransitStopDetails> stopDetailsResponse = await _networkService.GetStopDetails(args.StopSelected.GtfsId, DateTime.Now);
            IsDetailsLoading = false;
            
            if(stopDetailsResponse.IsFailure)
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

        public async Task<ApiResult<IEnumerable<TransitStop>>> SearchStopsAsync(string searchText, CancellationToken token)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                StopsResultList.Clear();
                return ApiResult<IEnumerable<TransitStop>>.Fail;
            }

            var response = await _networkService.GetStopsAsync(searchText, token);
            if (response.IsFailure)
            {                
                StopsResultList.Clear();
                return response;
            }
            if (token.IsCancellationRequested)
            {
                StopsResultList.Clear();
                return ApiResult<IEnumerable<TransitStop>>.Fail;
            }

            StopsResultList = new ObservableCollection<StopSearchElementViewModel>(
                response.Result.Select(x => new StopSearchElementViewModel(x, _messenger)));

            return response;
        }

        public async Task<ApiResult<IEnumerable<TransitStop>>> UpdateNearbyPlacesAsync(Geocircle circle, CancellationToken token)
        {
            ApiResult<IEnumerable<TransitStop>> response = await _networkService.GetStopsByBoundingRadius(
               (float)circle.Center.Latitude,
               (float)circle.Center.Longitude,
               (int)circle.Radius,
               token
           );

            if (response.IsFailure)
            {
                return response;
            }
            if (token.IsCancellationRequested)
            {
                return ApiResult<IEnumerable<TransitStop>>.Fail;
            }

            StopsResultList = new ObservableCollection<StopSearchElementViewModel>(response.Result.Select(x => new StopSearchElementViewModel(x, _messenger)));
            return response;
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
