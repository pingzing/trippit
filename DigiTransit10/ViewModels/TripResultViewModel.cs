using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;
using Template10.Common;
using DigiTransit10.Models.TripPlanStrip;
using DigiTransit10.Models;
using DigiTransit10.Localization.Strings;
using GalaSoft.MvvmLight.Command;
using System;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;

        private RelayCommand<ItineraryModel> _showTripDetailsCommand;
        public RelayCommand<ItineraryModel> ShowTripDetailsCommand => new RelayCommand<ItineraryModel>(ShowTripDetails);

        private RelayCommand<ItineraryModel> _showTripOnMapCommand;
        public RelayCommand<ItineraryModel> ShowTripOnMapCommand => new RelayCommand<ItineraryModel>(ShowTripOnMap);

        public ObservableCollection<ItineraryModel> _tripResults = new ObservableCollection<ItineraryModel>();
        public ObservableCollection<ItineraryModel> TripResults
        {
            get { return _tripResults; }
            set { Set(ref _tripResults, value); }
        }

        private string _fromName;
        public string FromName
        {
            get { return _fromName?.ToUpperInvariant(); }
            set { Set(ref _fromName, value); }
        }

        private string _toName;
        public string ToName
        {
            get { return _toName?.ToUpperInvariant(); }
            set { Set(ref _toName, value); }
        }

        public TripResultViewModel(INetworkService networkService, IMessenger messengerService)
        {
            _networkService = networkService;
            _messengerService = messengerService;

            _messengerService.Register<MessageTypes.PlanFoundMessage>(this, PlanFound);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {            
            await Task.CompletedTask;
        }

        private void PlanFound(MessageTypes.PlanFoundMessage _)
        {
            if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                return;
            }
            var plan = BootStrapper.Current.SessionState[NavParamKeys.PlanResults] as TripPlan;            
            if (plan?.ApiPlan?.Itineraries == null)
            {
                return;
            }
                                    
            TripResults.Clear();

            FromName = plan.StartingPlaceName ?? AppResources.TripPlanStrip_StartingPlaceDefault;
            ToName = plan.EndingPlaceName ?? AppResources.TripPlanStrip_EndPlaceDefault;
            foreach (var itinerary in plan.ApiPlan.Itineraries)
            {
                TripResults.Add(new ItineraryModel
                {
                    BackingItinerary = itinerary,
                    StartingPlaceName = plan.StartingPlaceName ?? AppResources.TripPlanStrip_StartingPlaceDefault,
                    EndingPlaceName = plan.EndingPlaceName ?? AppResources.TripPlanStrip_EndPlaceDefault
            });
            }
        }

        private void ShowTripDetails(ItineraryModel obj)
        {
            throw new NotImplementedException();
        }

        private void ShowTripOnMap(ItineraryModel obj)
        {
            
        }
    }
}
