using System;
using DigiTransit10.Helpers;
using DigiTransit10.Services;
using Template10.Common;
using Template10.Mvvm;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Localization.Strings;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;
using System.Threading;
using DigiTransit10.Models.Geocoding;
using System.Collections.Generic;

namespace DigiTransit10.ViewModels
{
    public class TripFormViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly Services.SettingsServices.SettingsService _settingsService;
        private readonly IMessenger _messengerService;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private bool? _isArrivalChecked = false;
        public bool? IsArrivalChecked
        {
            get { return _isArrivalChecked; }
            set { Set(ref _isArrivalChecked, value); }
        }

        private bool? _isDepartureChecked = true;
        public bool? IsDepartureChecked
        {
            get { return _isDepartureChecked; }
            set { Set(ref _isDepartureChecked, value); }
        }

        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;
        public TimeSpan SelectedTime
        {
            get { return _selectedTime; }
            set { Set(ref _selectedTime, value); }
        }

        private DateTimeOffset _selectedDate = DateTime.Now;
        public DateTimeOffset SelectedDate
        {
            get { return _selectedDate; }
            set { Set(ref _selectedDate, value); }
        }

        private Place _fromPlace;
        public Place FromPlace
        {
            get { return _fromPlace; }
            set { Set(ref _fromPlace, value); }
        }

        private Place _toPlace;
        public Place ToPlace
        {
            get { return _toPlace; }
            set { Set(ref _toPlace, value); }
        }

        private readonly RelayCommand _planTripNarrowViewCommand = null;
        public RelayCommand PlanTripNarrowViewCommand => _planTripNarrowViewCommand ?? (new RelayCommand(PlanTripNarrowView));

        private readonly RelayCommand _planTripWideViewCommand = null;
        public RelayCommand PlanTripWideViewCommand => _planTripWideViewCommand ?? (new RelayCommand(PlanTripWideView));

        public TripFormViewModel(INetworkService netService, IMessenger messengerService, Services.SettingsServices.SettingsService settings)
        {
            _networkService = netService;
            _settingsService = settings;
            _messengerService = messengerService;
        }

        private async Task PlanTrip()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            _cts = new CancellationTokenSource();

            Views.Busy.SetBusy(true, AppResources.TripFrom_GettingsAddresses);

            List<Place> places = new List<Place>();
            places.Add(FromPlace);
            places.Add(ToPlace);

            places = await ResolvePlaces(places, _cts.Token);
            FromPlace = places[0]; //todo: these will be replaced with some kind of list and loop when we move to "arbitrary # of legs" style input
            ToPlace = places[1]; // but for now, magic numbers wheee

            ApiCoordinates fromCoords = new ApiCoordinates { Lat = FromPlace.Lat, Lon = FromPlace.Lon };
            ApiCoordinates toCoords = new ApiCoordinates { Lat = ToPlace.Lat, Lon = ToPlace.Lon };
            BasicTripDetails details = new BasicTripDetails(
                fromCoords,
                toCoords,
                SelectedTime,
                SelectedDate.DateTime,
                IsArrivalChecked == true
            );

            Views.Busy.SetBusy(true, AppResources.TripForm_PlanningTrip);

            var result = await _networkService.PlanTrip(details);
            if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                BootStrapper.Current.SessionState.Add(NavParamKeys.PlanResults, result);
            }
            else
            {
                BootStrapper.Current.SessionState.Remove(NavParamKeys.PlanResults);
                BootStrapper.Current.SessionState.Add(NavParamKeys.PlanResults, result);
            }
            Views.Busy.SetBusy(false);

        }

        private async Task<List<Place>> ResolvePlaces(List<Place> places, CancellationToken token)
        {
            List<Task<bool>> getAddressTasks = new List<Task<bool>>();
            for (int i = places.Count - 1; i >= 0; i--)
            {                
                int idx = i; //capturing this in the closure so it doesn't get changed out from under us in the continuation
                if(places[idx].Type != ModelEnums.PlaceType.NameOnly)
                {
                    continue;
                }
                Task<GeocodingResponse> task = _networkService.SearchAddress(places[idx].Name, token);
                getAddressTasks.Add(task.ContinueWith(resp =>
                {
                    if (resp.Result == null || resp.Result.Features.Length == 0)
                    {
                        return false;
                    }
                    places[idx] = new Place
                    {
                        Lon = (float)task.Result.Features[0].Geometry.Coordinates[0],
                        Lat = (float)task.Result.Features[0].Geometry.Coordinates[1],
                        Name = task.Result.Features[0].Properties.Name,
                        Type = ModelEnums.PlaceType.Address,
                        Id = task.Result.Features[0].Properties.Id,
                        Confidence = task.Result.Features[0].Properties.Confidence
                    };
                    return true;
                }));
            }

            await Task.WhenAll(getAddressTasks);

            return places;
        }

        private async void PlanTripNarrowView()
        {
            await PlanTrip();
            _messengerService.Send<string>(Constants.NarrowKey, MessageTypes.PlanFoundMessage);
        }

        private async void PlanTripWideView()
        {
            await PlanTrip();
            _messengerService.Send<string>(Constants.WideKey, MessageTypes.PlanFoundMessage);
        }
    }
}
