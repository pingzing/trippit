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
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Styles;
using System;
using DigiTransit10.Services.SettingsServices;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;        

        public RelayCommand<ItineraryModel> ShowTripDetailsCommand => new RelayCommand<ItineraryModel>(ShowTripDetails);
        public RelayCommand GoBackToTripListCommand => new RelayCommand(GoBackToTripList);
        public RelayCommand<ItineraryModel> SaveRouteCommand => new RelayCommand<ItineraryModel>(SaveRoute); 

        private ObservableCollection<ItineraryModel> _tripResults = new ObservableCollection<ItineraryModel>();
        public ObservableCollection<ItineraryModel> TripResults
        {
            get { return _tripResults; }
            set { Set(ref _tripResults, value); }
        }

        private IList<ColoredMapLinePoint> _coloredMapLinePoints = new List<ColoredMapLinePoint>();
        public IList<ColoredMapLinePoint> ColoredMapLinePoints
        {
            get { return _coloredMapLinePoints; }
            set { Set(ref _coloredMapLinePoints, value); }
        }

        private IEnumerable<IMapPoi> _mapStops = new List<IMapPoi>();
        public IEnumerable<IMapPoi> MapStops
        {
            get { return _mapStops; }
            set { Set(ref _mapStops, value); }
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

        public TripResultViewModel(INetworkService networkService, IMessenger messengerService, SettingsService settings)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = settings;

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

        private async void PlanFound(MessageTypes.PlanFoundMessage _)
        {
            if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                return;
            }
            var foundPlan = BootStrapper.Current.SessionState[NavParamKeys.PlanResults] as TripPlan;
            if (foundPlan?.ApiPlan?.Itineraries == null)
            {
                return;
            }

            //todo: leaking abstraction here, see if we can move this to the view
            Task waitForAnimationTask = null;
            if (IsInDetailedState)
            {
                GoBackToTripList();
                waitForAnimationTask = Task.Delay(450);
            }
            TripResults.Clear();

            FromName = foundPlan.StartingPlaceName ?? AppResources.TripPlanStrip_StartingPlaceDefault;
            ToName = foundPlan.EndingPlaceName ?? AppResources.TripPlanStrip_EndPlaceDefault;

            // Give the control enough time to animate back from the DetailedState, 
            // so that when the TripPlanStrip does it's second render pass, it gets accurate values.
            if (waitForAnimationTask != null) await waitForAnimationTask;

            foreach (var itinerary in foundPlan.ApiPlan.Itineraries)
            {
                var model = new ItineraryModel
                {
                    BackingItinerary = itinerary,
                    StartingPlaceName = foundPlan.StartingPlaceName ?? AppResources.TripPlanStrip_StartingPlaceDefault,
                    EndingPlaceName = foundPlan.EndingPlaceName ?? AppResources.TripPlanStrip_EndPlaceDefault
                };
                TripResults.Add(model);
            }
        }

        private void ShowTripDetails(ItineraryModel model)
        {
            List<DetailedTripListLeg> legs = model.BackingItinerary.Legs
                .Select(x => DetailedTripListLeg.FromApiLeg(x))
                .ToList();

            //Rename the From/To for the First/Last legs, as the API doesn't return the names for those
            legs.First().FromName = model.StartingPlaceName;
            DetailedTripListLeg leg = DetailedTripListLeg.ApiLegToEndLeg(model.BackingItinerary.Legs.Last());
            leg.ToName = model.EndingPlaceName;
            legs.Add(leg);

            SelectedDetailLegs = legs;

            ColoredMapLinePoints = model.BackingItinerary.Legs
                .SelectMany(y => 
                    GooglePolineDecoder.Decode(y.LegGeometry.Points)
                    .Select(x => {
                        if(y.Mode == ApiEnums.ApiMode.Walk)
                        {
                            return new ColoredMapLinePoint(x, HslColors.GetModeColor(y.Mode.Value), true);
                        }
                        else
                        {
                            return new ColoredMapLinePoint(x, HslColors.GetModeColor(y.Mode.Value));
                        }
                    })
                ).ToList();

            var stops = new List<IMapPoi>();
            ApiPlace startPoint = model.BackingItinerary.Legs.First().From;
            startPoint.Name = model.StartingPlaceName;
            stops.Add(startPoint);

            stops.AddRange(model.BackingItinerary.Legs.Select(x => x.To));
            stops.Last().Name = model.EndingPlaceName;

            MapStops = stops;

            _messengerService.Send(new MessageTypes.ViewPlanDetails(model));
            IsInDetailedState = true;
        }

        private void SaveRoute(ItineraryModel routeToSave)
        {
            var route = new FavoriteRoute
            {
                FontIconGlyph = FontIconGlyphs.FilledStar,
                IconFontFace = Constants.SymbolFontFamily,
                UserChosenName = $"{routeToSave.StartingPlaceName} → {routeToSave.EndingPlaceName}",
            };            

            var places = new List<FavoriteRoutePlace>();
            ApiLeg fromPlace = routeToSave.BackingItinerary.Legs.First();
            ApiLeg toPlace = routeToSave.BackingItinerary.Legs.Last();
            places.Add(new FavoriteRoutePlace
            {
                Lat = fromPlace.From.Lat,
                Lon = fromPlace.From.Lon,
                Name = routeToSave.StartingPlaceName
            });
            places.Add(new FavoriteRoutePlace
            {
                Lat = toPlace.To.Lat,
                Lon = toPlace.To.Lon,
                Name = routeToSave.EndingPlaceName
            });

            route.RoutePlaces = places;
            _settingsService.AddFavorite(route);
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
            if (!suspending)
            {
                BootStrapper.BackRequested -= BootStrapper_BackRequested;
            }
            await Task.CompletedTask;
        }
    }
}
