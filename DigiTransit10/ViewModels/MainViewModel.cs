using DigiTransit10.Helpers;
using DigiTransit10.Services;
using DigiTransit10.Views;
using GalaSoft.MvvmLight.Messaging;
using MetroLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Models;
using GalaSoft.MvvmLight.Command;
using System;

namespace DigiTransit10.ViewModels
{

    public sealed class MainViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly ILogger _logger;
        private readonly Services.SettingsServices.SettingsService _settingsService;

        private TransitTrafficAlertComparer _transitTrafficAlertComparer;

        private List<TransitTrafficAlert> _trafficAlerts = new List<TransitTrafficAlert>();
        public List<TransitTrafficAlert> TrafficAlerts
        {
            get { return _trafficAlerts; }
            set
            {
                Set(ref _trafficAlerts, value);
                RaisePropertyChanged(nameof(AlertCount));
            }
        }

        public int AlertCount => _trafficAlerts.Count;

        private bool _areAlertsFresh = true;
        public bool AreAlertsFresh
        {
            get { return _areAlertsFresh; }
            set { Set(ref _areAlertsFresh, value); }
        }

        private RelayCommand _viewAlertsCommand;
        public RelayCommand ViewAlertsCommand => _viewAlertsCommand ?? (_viewAlertsCommand = new RelayCommand(ViewAlerts));

        public MainViewModel(INetworkService networkService, IMessenger messengerService,
            Services.SettingsServices.SettingsService settings, ILogger logger)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = settings;
            _logger = logger;

            _messengerService.Register<MessageTypes.PlanFoundMessage>(this, PlanFound);
            _messengerService.Register<MessageTypes.LineSearchRequested>(this, SearchLine);
            _transitTrafficAlertComparer = new TransitTrafficAlertComparer();             

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

        private void SearchLine(MessageTypes.LineSearchRequested lineSearchMessage)
        {            
            // We don't want to intercept messages coming from the SearchPage!
            if (lineSearchMessage.Source == typeof(TripResultViewModel))
            {
                NavigationService.NavigateAsync(typeof(SearchPage), lineSearchMessage);
            }
        }

        private async void ViewAlerts()
        {
            AreAlertsFresh = false;
            await NavigationService.NavigateAsync(typeof(AlertsPage), TrafficAlerts);            
        }
        
        private async Task UpdateAlerts()
        {
            ApiResult<IEnumerable<TransitTrafficAlert>> response = await _networkService.GetTrafficAlertsAsync();
            if (response.HasResult)
            {
                List<TransitTrafficAlert> newAlerts = response
                    .Result
                    .Distinct(_transitTrafficAlertComparer) // Sometimes we get a bunch of duplicate alerts that have different IDs, but no other difference
                    .Where(x => !String.IsNullOrWhiteSpace(x.DescriptionText.Text)) // or empty alerts
                    .ToList();                
                AreAlertsFresh = newAlerts.Any(newAlert => TrafficAlerts.All(oldAlert => oldAlert.Id != newAlert.Id));
                TrafficAlerts = newAlerts;
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {            
            if (suspensionState.Any())
            {
                //pull cached values in from the suspensionState dict
            }
            await TripFormViewModel.OnNavigatedToAsync(parameter, mode, suspensionState);

            await UpdateAlerts();
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

    internal class TransitTrafficAlertComparer : IEqualityComparer<TransitTrafficAlert>
    {
        public bool Equals(TransitTrafficAlert x, TransitTrafficAlert y)
        {
            return x.DescriptionText.Language == y.DescriptionText.Language
                && x.DescriptionText.Text == y.DescriptionText.Text
                && x.StartDate == y.StartDate
                && x.EndDate == y.EndDate;
        }

        public int GetHashCode(TransitTrafficAlert obj)
        {
            unchecked
            {
                var hashCode = obj.DescriptionText.Language.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.DescriptionText.Text?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.StartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.EndDate.GetHashCode();
                return hashCode;
            }
        }
    }
}

