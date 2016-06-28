using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Xaml.Controls;
using DigiTransit10.Services;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using GalaSoft.MvvmLight.Command;

namespace DigiTransit10.ViewModels
{
    public class TripFormViewModel : ViewModelBase
    {
        public readonly INetworkService _networkService;
        public readonly ISettingsService _settingsService;

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

        public TripFormViewModel(INetworkService netService)
        {
            _networkService = netService;
            _settingsService = ((App) BootStrapper.Current).Locator.GetLocalSettingsService();
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
            var result = await _networkService.PlanTrip(details);
        }
    }
}
