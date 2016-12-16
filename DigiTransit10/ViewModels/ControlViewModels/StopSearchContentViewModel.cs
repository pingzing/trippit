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

        public RelayCommand LoadedCommand => new RelayCommand(Loaded);
        public RelayCommand UnloadedCommand => new RelayCommand(Unloaded);

        private ObservableCollection<StopSearchElementViewModel> _stopsResultList = new ObservableCollection<StopSearchElementViewModel>();
        public ObservableCollection<StopSearchElementViewModel> StopsResultList
        {
            get { return _stopsResultList; }
            set { Set(ref _stopsResultList, value); }
        }

        private ObservableCollection<TransitLine> _linesAtStop = new ObservableCollection<TransitLine>();
        public ObservableCollection<TransitLine> LinesAtStop
        {
            get { return _linesAtStop; }
            set { Set(ref _linesAtStop, value); }
        }

        public StopSearchContentViewModel(IMessenger messenger, INetworkService network)
        {
            _messenger = messenger;
            _messenger.Register<MessageTypes.ViewStopDetails>(this, SwitchToDetailedView);
            _networkService = network;
        }

        private async void SwitchToDetailedView(MessageTypes.ViewStopDetails args)
        {
            RaiseStateChanged(StopSearchState.Details);            
            //await GetRoutesThatPassThroughStop(args.StopSelected);
            //insert those routes into LinesAtStop
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
                RaiseStateChanged(StopSearchState.Overview);
            }
        }

        public override event VmStateChangeHandler VmStateChangeRequested;
        private void RaiseStateChanged(StopSearchState newState)
        {
            _currentState = newState;
            VmStateChangeRequested?.Invoke(this, new VmStateChangedEventArgs(newState.ToString()));
        }
    }
}
