using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Xaml.Controls;
using DigiTransit10.Helpers;
using DigiTransit10.Services;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DigiTransit10.ViewModels
{
    public class TripFormViewModel : ViewModelBaseEx
    {
        private readonly INetworkService _networkService;
        private readonly ISettingsService _settingsService;
        private readonly IMessenger _messengerService;

        private bool _isTimeTypeArrival = false;
        public bool IsTimeTypeArrival
        {
            get { return _isTimeTypeArrival; }
            set { Set(ref _isTimeTypeArrival, value); }
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

        private readonly RelayCommand _planTripCommand = null;
        public RelayCommand PlanTripCommand => _planTripCommand ?? (new RelayCommand(PlanTrip));        

        public TripFormViewModel(INetworkService netService, IMessenger messengerService)
        {
            _networkService = netService;
            _settingsService = ((App) BootStrapper.Current).Locator.GetLocalSettingsService();
            _messengerService = messengerService;
        }

        private async void PlanTrip()
        {
            ApiCoordinates DEBUG_FROM_COORD = new ApiCoordinates {Lat = 60.23f, Lon = 25.12f,};
            ApiCoordinates DEBUG_TO_COORD = new ApiCoordinates {Lat = 60.17f, Lon = 24.94f};
            BasicTripDetails details = new BasicTripDetails(
                DEBUG_FROM_COORD, 
                DEBUG_TO_COORD, 
                SelectedTime, 
                SelectedDate.DateTime, 
                IsTimeTypeArrival
            );
            Views.Busy.SetBusy(true, "Planning...");
            var result = await _networkService.PlanTrip(details);
            if (!SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                SessionState.Add(NavParamKeys.PlanResults, result);
            }
            else
            {
                SessionState[NavParamKeys.PlanResults] = result;
            }            
            Views.Busy.SetBusy(false);
            _messengerService.Send<object>(null, MessageTypes.PlanFoundMessage);
        }        
    }
}
