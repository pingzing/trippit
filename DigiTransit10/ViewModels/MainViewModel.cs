using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using DigiTransit10.Services;
using Template10.Common;

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
                //set up design-time values
            }
        }        

        public TripFormViewModel TripFormViewModel => ((App) BootStrapper.Current).Locator.TripForm;
        public TripResultViewModel TripResultViewModel => ((App) BootStrapper.Current).Locator.TripResult;               

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                //pull cached values in from the suspensionState dict
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {                        
            if (suspending)
            {
                //store cacheable values into the suspensionState dict
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

