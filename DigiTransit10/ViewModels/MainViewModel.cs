using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using DigiTransit10.Services;

namespace DigiTransit10.ViewModels
{    

    public class MainViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;

        public MainViewModel(INetworkService networkService)
        {
            _networkService = networkService;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
        }

        string _value = "Gas";
        public string Value { get { return _value; } set { Set(ref _value, value); } }

        private RelayCommand<string> _getStopDetailsCommand = null;
        public RelayCommand<string> GetStopDetailsCommand => _getStopDetailsCommand ?? (new RelayCommand<string>(GetStopDetails));

        private async void GetStopDetails(string stopId)
        {
            int numericStopId = int.Parse(stopId);
            Value = await _networkService.GetStop(numericStopId);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }        

    }
}

