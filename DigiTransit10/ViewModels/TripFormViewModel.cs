using System;
using DigiTransit10.Helpers;
using DigiTransit10.Services;
using Template10.Common;
using Template10.Mvvm;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;

namespace DigiTransit10.ViewModels
{
    public class TripFormViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly Services.SettingsServices.SettingsService _settingsService;
        private readonly IMessenger _messengerService;

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
            ApiCoordinates DEBUG_FROM_COORD = new ApiCoordinates { Lat = 60.23f, Lon = 25.12f, };
            ApiCoordinates DEBUG_TO_COORD = new ApiCoordinates { Lat = 60.17f, Lon = 24.94f };
            BasicTripDetails details = new BasicTripDetails(
                DEBUG_FROM_COORD,
                DEBUG_TO_COORD,
                SelectedTime,
                SelectedDate.DateTime,
                IsArrivalChecked == true
            );
            Views.Busy.SetBusy(true, "Planning...");
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
