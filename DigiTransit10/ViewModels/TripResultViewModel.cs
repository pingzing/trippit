using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBaseEx
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;

        public ObservableCollection<ApiItinerary> _tripResults = new ObservableCollection<ApiItinerary>();
        public ObservableCollection<ApiItinerary> TripResults
        {
            get { return _tripResults; }
            set { Set(ref _tripResults, value); }
        }  

        public TripResultViewModel(INetworkService networkService, IMessenger messengerService)
        {
            _networkService = networkService;
            _messengerService = messengerService;

            _messengerService.Register<object>(this, MessageTypes.PlanFoundMessage, PlanFound);                        
        }        

        private void PlanFound(object obj)
        {
            if (!SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                return;
            }
            var plan = SessionState[NavParamKeys.PlanResults] as ApiPlan;            
            if (plan?.Itineraries == null)
            {
                return;
            }

            TripResults.Clear();
            foreach (var itinerary in plan.Itineraries)
            {
                TripResults.Add(itinerary);
            }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            return base.OnNavigatedToAsync(parameter, mode, state);
            NavigationService.FrameFacade.BackRequested += FrameFacade_BackRequested;
        }

        private void FrameFacade_BackRequested(object sender, Template10.Common.HandledEventArgs e)
        {
            e.Handled = true;
            _messengerService.Send<object>(null, MessageTypes.GoBackToTripFormMessage);
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            return base.OnNavigatedFromAsync(pageState, suspending);
            NavigationService.FrameFacade.BackRequested -= FrameFacade_BackRequested;

        }
    }
}
