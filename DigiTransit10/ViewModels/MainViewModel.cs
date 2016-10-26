using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Messaging;
using Template10.Common;
using DigiTransit10.Views;
using Windows.UI.Xaml.Media.Animation;
using MetroLog;

namespace DigiTransit10.ViewModels
{

    public sealed class MainViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly ILogger _logger;
        private readonly Services.SettingsServices.SettingsService _settingsService;

        public MainViewModel(INetworkService networkService, IMessenger messengerService,
            Services.SettingsServices.SettingsService settings, ILogger logger)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = settings;
            _logger = logger;            

            _messengerService.Register<MessageTypes.PlanFoundMessage>(this, PlanFound);

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                //set up design-time values
            }
        }


        public TripFormViewModel TripFormViewModel
        {
            get
            {
                TripFormViewModel tripForm = ((App) BootStrapper.Current).Locator.TripForm;
                tripForm.SessionState = SessionState;
                return tripForm;
            }
        }
        public TripResultViewModel TripResultViewModel => ((App) BootStrapper.Current).Locator.TripResult;        

        private void PlanFound(MessageTypes.PlanFoundMessage planFoundMessage)
        {            
            if(planFoundMessage.VisualState == MessageTypes.VisualStateType.Narrow)
            {                
                NavigationService.NavigateAsync(typeof(TripResultPage));
            }
        }        

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                //pull cached values in from the suspensionState dict
            }
            await TripFormViewModel.OnNavigatedToAsync(parameter, mode, suspensionState);
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //store cacheable values into the suspensionState dict
            }
            await TripFormViewModel.OnNavigatedFromAsync(suspensionState, suspending);
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await TripFormViewModel.OnNavigatingFromAsync(args);
            await Task.CompletedTask;
        }

    }
}

