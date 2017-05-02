using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Localization.Strings;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Services;
using DigiTransit10.Services.SettingsServices;
using DigiTransit10.Styles;
using DigiTransit10.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;
        private readonly IFavoritesService _favoritesService;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        public RelayCommand<TripItinerary> ShowTripDetailsCommand => new RelayCommand<TripItinerary>(ShowTripDetails);
        public RelayCommand GoBackToTripListCommand => new RelayCommand(GoBackToTripList);
        public RelayCommand<TripItinerary> SaveRouteCommand => new RelayCommand<TripItinerary>(SaveRoute);
        public RelayCommand<string> SearchForLineCommand => new RelayCommand<string>(SearchForLine);        

        private ObservableCollection<TripItinerary> _tripResults = new ObservableCollection<TripItinerary>();
        public ObservableCollection<TripItinerary> TripResults
        {
            get { return _tripResults; }
            set { Set(ref _tripResults, value); }
        }

        private ObservableCollection<ColoredMapLine> _coloredMapLines = new ObservableCollection<ColoredMapLine>();
        public ObservableCollection<ColoredMapLine> ColoredMapLines
        {
            get { return _coloredMapLines; }
            set { Set(ref _coloredMapLines, value); }
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

        private List<TripLeg> _selectedDetailLegs = null;
        public List<TripLeg> SelectedDetailLegs
        {
            get { return _selectedDetailLegs; }
            set { Set(ref _selectedDetailLegs, value); }
        }        

        public TripResultViewModel(INetworkService networkService, IMessenger messengerService, SettingsService settings,
            IFavoritesService favorites, IFileService fileService, ILogger logger)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = settings;
            _favoritesService = favorites;
            _fileService = fileService;
            _logger = logger;

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
            BootStrapper.Current.SessionState.Remove(NavParamKeys.PlanResults);
            if (foundPlan?.PlanItineraries == null)
            {
                return;
            }
            TripResults.Clear();
            FromName = foundPlan.StartingPlaceName ?? AppResources.TripPlanStrip_StartingPlaceDefault;
            ToName = foundPlan.EndingPlaceName ?? AppResources.TripPlanStrip_EndPlaceDefault;

            //----todo: leaking abstraction here, see if we can move this to the view
            // Give the control enough time to animate back from the DetailedState, 
            // so that when the TripPlanStrip does it's second render pass, it gets accurate values.            
            if (IsInDetailedState)
            {
                GoBackToTripList();
                await Task.Delay(450);
            }
            //----end todo            
            foreach (TripItinerary itinerary in foundPlan.PlanItineraries)
            {
                TripResults.Add(itinerary);
            }
        }

        private void ShowTripDetails(TripItinerary model)
        {
            List<Guid> legIds = new List<Guid>();
            for (int i = 0; i <= model.ItineraryLegs.Count - 1; i++)
            {
                legIds.Add(Guid.NewGuid());
            }

            SelectedDetailLegs = model.ItineraryLegs
                .Zip(legIds, (x, id) => { x.TemporaryId = id; return x; })
                .ToList();

            ColoredMapLines.Clear();
            int legIndex = 0;
            foreach(TripLeg leg in model.ItineraryLegs)
            {
                List<ColoredMapLinePoint> coloredPoints = GooglePolineDecoder.Decode(leg.LegGeometryString)
                    .Select(x =>
                    {
                        if (leg.Mode == ApiEnums.ApiMode.Walk)
                        {
                            return new ColoredMapLinePoint(x, HslColors.GetModeColor(leg.Mode), true);
                        }
                        else
                        {
                            return new ColoredMapLinePoint(x, HslColors.GetModeColor(leg.Mode));
                        }
                    })
                    .ToList();
                var mapLine = new ColoredMapLine(coloredPoints, legIds[legIndex]);
                ColoredMapLines.Add(mapLine);
                legIndex++;
            }

            var stops = new List<IMapPoi>();
            stops.AddRange(model.ItineraryLegs.Zip(legIds, (x, id) => {
                IMapPoi poi = x.StartPlaceToPoi();
                poi.Id = id;
                return poi;
            }));
            IMapPoi endPoi = model.ItineraryLegs.Last().EndPlaceToPoi();
            endPoi.Id = legIds.Last();
            stops.Add(endPoi);

            MapStops = stops;

            _messengerService.Send(new MessageTypes.ViewPlanDetails(model));
            IsInDetailedState = true;
        }

        private void SaveRoute(TripItinerary routeToSave)
        {
            var route = new FavoriteRoute
            {
                FontIconGlyph = FontIconGlyphs.FilledStar,
                Id = Guid.NewGuid(),
                IconFontFace = Constants.SegoeMdl2FontName,
                IconFontSize = Constants.SymbolFontSize,
                UserChosenName = $"{routeToSave.StartingPlaceName} → {routeToSave.EndingPlaceName}",
            };

            TripLeg startPlace = routeToSave.ItineraryLegs.First();
            TripLeg endPlace = routeToSave.ItineraryLegs.Last();
            var places = new List<SimpleFavoritePlace>();
            places.Add(new SimpleFavoritePlace
            {
                Lat = startPlace.StartCoords.Latitude,
                Lon = startPlace.StartCoords.Longitude,
                Name = routeToSave.StartingPlaceName
            });
            places.Add(new SimpleFavoritePlace
            {
                Lat = endPlace.EndCoords.Latitude,
                Lon = endPlace.EndCoords.Longitude,
                Name = routeToSave.EndingPlaceName
            });

            route.RoutePlaces = places;
            route.RouteGeometryStrings = routeToSave.RouteGeometryStrings.ToList();
            _favoritesService.AddFavorite(route);
        }

        private void SearchForLine(string lineId)
        {
            _messengerService.Send(new MessageTypes.LineSearchRequested(lineId, MessageTypes.LineSearchType.ById));
        }

        private void GoBackToTripList()
        {
            _messengerService.Send(new MessageTypes.ViewPlanStrips());
            IsInDetailedState = false;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            BootStrapper.BackRequested += BootStrapper_BackRequested;
            if (suspensionState.ContainsKey(SuspensionKeys.TripResults_HasSavedState)
                && (bool)suspensionState[SuspensionKeys.TripResults_HasSavedState])
            {
                IStorageFile tripResultCacheFile = await _fileService.GetTempFileAsync(
                    SuspensionKeys.TripResults_HasSavedState, CreationCollisionOption.OpenIfExists);

                using(Stream fileInStream = await tripResultCacheFile.OpenStreamForReadAsync())
                using (var gzip = new GZipStream(fileInStream, CompressionMode.Decompress))
                {
                    TripItinerary[] tripsArray = gzip.DeseriaizeJsonFromStream<TripItinerary[]>();
                    TripResults = new ObservableCollection<TripItinerary>(tripsArray);
                }

                FromName = (string)suspensionState[SuspensionKeys.TripResults_FromName];
                ToName = (string) suspensionState[SuspensionKeys.TripResults_ToName];

                suspensionState.Remove(SuspensionKeys.TripResults_HasSavedState);
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                IStorageFile tripResultCacheFile = await _fileService.GetTempFileAsync(
                    SuspensionKeys.TripResults_HasSavedState, CreationCollisionOption.ReplaceExisting);

                using (Stream fileStream = await tripResultCacheFile.OpenStreamForWriteAsync())
                using (GZipStream jsonStream = new GZipStream(fileStream, CompressionLevel.Fastest))
                {
                    jsonStream.SerializeJsonToStream(TripResults.ToArray());
                }

                //and make a note of it in the suspension dict:
                suspensionState.AddOrUpdate(SuspensionKeys.TripResults_HasSavedState, true);
                suspensionState.AddOrUpdate(SuspensionKeys.TripResults_FromName, FromName);
                suspensionState.AddOrUpdate(SuspensionKeys.TripResults_ToName, ToName);
            }
            else //Don't want to unhook the "back-returns from Detailed View" behavior if the user is just switching apps.
            {
                BootStrapper.BackRequested -= BootStrapper_BackRequested;
            }
            await Task.CompletedTask;
        }
    }
}
