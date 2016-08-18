using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;
using Template10.Common;
using DigiTransit10.Models;
using DigiTransit10.Localization.Strings;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Windows.Devices.Geolocation;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;

        public RelayCommand<ItineraryModel> ShowTripDetailsCommand => new RelayCommand<ItineraryModel>(ShowTripDetails);
        public RelayCommand GoBackToTripListCommand => new RelayCommand(GoBackToTripList);

        private ObservableCollection<ItineraryModel> _tripResults = new ObservableCollection<ItineraryModel>();
        public ObservableCollection<ItineraryModel> TripResults
        {
            get { return _tripResults; }
            set { Set(ref _tripResults, value); }
        }

        private IEnumerable<BasicGeoposition> _mapLinePoints = new List<BasicGeoposition>();
        public IEnumerable<BasicGeoposition> MapLinePoints
        {
            get { return _mapLinePoints; }
            set { Set(ref _mapLinePoints, value); }
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

        private bool _isinDetailedState = false;
        public bool IsInDetailedState
        {
            get { return _isinDetailedState; }
            set { Set(ref _isinDetailedState, value); }
        }

        private List<DetailedTripListLeg> _selectedDetailLegs = null;
        public List<DetailedTripListLeg> SelectedDetailLegs
        {
            get { return _selectedDetailLegs; }
            set { Set(ref _selectedDetailLegs, value); }
        }

        public TripResultViewModel(INetworkService networkService, IMessenger messengerService)
        {
            _networkService = networkService;
            _messengerService = messengerService;

            _messengerService.Register<MessageTypes.PlanFoundMessage>(this, PlanFound);
        }

        private void BootStrapper_BackRequested(object sender, HandledEventArgs e)
        {
            if(IsInDetailedState)
            {
                e.Handled = true;
                GoBackToTripList();
            }
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
                var model = new ItineraryModel
                {
                    BackingItinerary = itinerary,
                    StartingPlaceName = plan.StartingPlaceName ?? AppResources.TripPlanStrip_StartingPlaceDefault,
                    EndingPlaceName = plan.EndingPlaceName ?? AppResources.TripPlanStrip_EndPlaceDefault
                };
                TripResults.Add(model);
            }
        }

        private void ShowTripDetails(ItineraryModel model)
        {
            SelectedDetailLegs = model.BackingItinerary.Legs.Select(x =>
            {
                var listLeg = DetailedTripListLeg.FromApiLeg(x);
                if (model.BackingItinerary.Legs.Last() == x)
                {
                    listLeg.IsEnd = true;
                    listLeg.ToName = model.EndingPlaceName;
                }
                return listLeg;
            }).ToList();

            MapLinePoints = model.BackingItinerary.Legs
                .SelectMany(x => GooglePolineDecoder.Decode(x.LegGeometry.Points));                

            _messengerService.Send(new MessageTypes.ViewPlanDetails(model));
            IsInDetailedState = true;
        }

        private void GoBackToTripList()
        {
            _messengerService.Send(new MessageTypes.ViewPlanStrips());
            IsInDetailedState = false;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            BootStrapper.BackRequested += BootStrapper_BackRequested;
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            BootStrapper.BackRequested -= BootStrapper_BackRequested;
            await Task.CompletedTask;
        }
    }
}
