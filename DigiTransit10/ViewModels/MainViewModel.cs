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

namespace DigiTransit10.ViewModels
{

    public sealed class MainViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly Services.SettingsServices.SettingsService _settingsService;

        public MainViewModel(INetworkService networkService, IMessenger messengerService, Services.SettingsServices.SettingsService settings)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = settings;           

            _messengerService.Register<MessageTypes.PlanFoundMessage>(this, PlanFound);            

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                //set up design-time values
            }
        }        

        public TripFormViewModel TripFormViewModel => ((App) BootStrapper.Current).Locator.TripForm;
        public TripResultViewModel TripResultViewModel => ((App) BootStrapper.Current).Locator.TripResult;
        public FavoritesViewModel FavoritesViewModel => ((App)BootStrapper.Current).Locator.Favorites;
        
        private void PlanFound(MessageTypes.PlanFoundMessage planFoundMessage)
        {
            if(!SessionState.ContainsKey(Constants.CurrentMainPageVisualStateKey))
            {
                throw new ArgumentNullException("CurrentMainPageVisualStateKey");
            }

            if((string)SessionState[Constants.CurrentMainPageVisualStateKey] == Constants.VisualStateNarrow)
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
            await Task.CompletedTask;
        }        

    }
}

