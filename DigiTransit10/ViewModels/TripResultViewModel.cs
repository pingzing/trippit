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
using Template10.Common;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBase
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

            _messengerService.Register<string>(this, MessageTypes.PlanFoundMessage, PlanFound);            
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            //Only fired when coming in via the narrow view.
            PlanFound(null);

            await Task.CompletedTask;
        }

        private void PlanFound(string obj)
        {
            if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                return;
            }
            var plan = BootStrapper.Current.SessionState[NavParamKeys.PlanResults] as ApiPlan;            
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
    }
}
