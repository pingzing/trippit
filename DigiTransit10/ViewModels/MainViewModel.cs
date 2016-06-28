using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using GalaSoft.MvvmLight.Command;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Messaging;
using Template10.Common;

namespace DigiTransit10.ViewModels
{    

    public sealed class MainViewModel : ViewModelBaseEx
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;

        public enum ActivePivotEnum
        {
            TripForm = 0,
            TripResults = 1
        };

        private int _activePivot = (int)ActivePivotEnum.TripForm;
        public int ActivePivot
        {
            get { return _activePivot; }
            set { Set(ref _activePivot, value); }
        }

        private bool _isPivotLocked = false;
        public bool IsPivotLocked
        {
            get { return _isPivotLocked; }
            set { Set(ref _isPivotLocked, value); }
        }

        public MainViewModel(INetworkService networkService, IMessenger messengerService)
        {
            _networkService = networkService;
            _messengerService = messengerService;

            _messengerService.Register<object>(this, MessageTypes.PlanFoundMessage, PlanFound);
            _messengerService.Register<object>(this, MessageTypes.GoBackToTripFormMessage, GoBackToTripForm);

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                //set up design-time values
            }
        }        

        public TripFormViewModel TripFormViewModel => ((App) BootStrapper.Current).Locator.TripForm;
        public TripResultViewModel TripResultViewModel => ((App) BootStrapper.Current).Locator.TripResult;

        //If we're in the narrow view state, change the active Pivot.
        private void PlanFound(object obj)
        {
            ActivePivot = (int) ActivePivotEnum.TripResults;
            IsPivotLocked = true;
        }

        private void GoBackToTripForm(object obj)
        {
            IsPivotLocked = false;
            ActivePivot = (int) ActivePivotEnum.TripForm;            
        }

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

