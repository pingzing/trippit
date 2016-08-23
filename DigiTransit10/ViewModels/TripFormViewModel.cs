using System;
using DigiTransit10.Helpers;
using DigiTransit10.Services;
using Template10.Common;
using Template10.Mvvm;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Localization.Strings;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;
using System.Threading;
using DigiTransit10.Models.Geocoding;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using static DigiTransit10.Models.ModelEnums;
using System.Text;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using static DigiTransit10.Helpers.Enums;
using Template10.Services.NavigationService;

namespace DigiTransit10.ViewModels
{
    public class TripFormViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly Services.SettingsServices.SettingsService _settingsService;
        private readonly IMessenger _messengerService;
        private readonly IGeolocationService _geolocationService;
        private readonly IDialogService _dialogService;

        private bool _isBusy;
        private string _currentBusyMessage;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        private bool _isUsingCurrentTime = true;
            public bool IsUsingCurrentTime
        {
            get { return _isUsingCurrentTime; }
            set { Set(ref _isUsingCurrentTime, value); }
        }

        private bool? _isArrivalChecked = false;
        public bool? IsArrivalChecked
        {
            get { return _isArrivalChecked; }
            set { Set(ref _isArrivalChecked, value); }
        }

        private bool? _isDepartureChecked = true;
        public bool? IsDepartureChecked
        {
            get { return _isDepartureChecked; }
            set { Set(ref _isDepartureChecked, value); }
        }        

        private bool _isTransitPanelVisible = false;
        public bool IsTransitPanelVisible
        {
            get { return _isTransitPanelVisible; }
            set { Set(ref _isTransitPanelVisible, value); }
        }

        private string _showHideTransitPanelText = AppResources.TripForm_MoreOptions;
        public string ShowHideTransitPanelText
        {
            get { return _showHideTransitPanelText; }
            set { Set(ref _showHideTransitPanelText, value); }
        }

        private string _showHideTransitPanelGlyph = FontIconGlyphs.BoldDownArrow;
        public string ShowHideTransitPanelGlyph
        {
            get { return _showHideTransitPanelGlyph; }
            set { Set(ref _showHideTransitPanelGlyph, value); }
        }

        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;
        public TimeSpan SelectedTime
        {
            get { return _selectedTime; }
            set { Set(ref _selectedTime, value); }
        }

        private DateTimeOffset _selectedDate = DateTime.Now;
        public DateTimeOffset SelectedDate
        {
            get { return _selectedDate; }
            set { Set(ref _selectedDate, value); }
        }

        private IPlace _fromPlace = new Place { Type = PlaceType.UserCurrentLocation, Name = AppResources.SuggestBoxHeader_MyLocation };
        public IPlace FromPlace
        {
            get { return _fromPlace; }
            set { Set(ref _fromPlace, value); }
        }

        private IPlace _toPlace;
        public IPlace ToPlace
        {
            get { return _toPlace; }
            set { Set(ref _toPlace, value); }
        }

        private bool? _isBusChecked = true;
        public bool? IsBusChecked
        {
            get { return _isBusChecked; }
            set { Set(ref _isBusChecked, value); }
        }

        private bool? _isTramChecked = true;
        public bool? IsTramChecked
        {
            get { return _isTramChecked; }
            set { Set(ref _isTramChecked, value); }
        }

        private bool? _isTrainChecked = true;
        public bool? IsTrainChecked
        {
            get { return _isTrainChecked; }
            set { Set(ref _isTrainChecked, value); }
        }

        private bool? _isMetroChecked = true;
        public bool? IsMetroChecked
        {
            get { return _isMetroChecked; }
            set { Set(ref _isMetroChecked, value); }
        }

        private bool? _isFerryChecked = false;
        public bool? IsFerryChecked
        {
            get { return _isFerryChecked; }
            set { Set(ref _isFerryChecked, value); }
        }

        private bool? _isBikeChecked = false;
        public bool? IsBikeChecked
        {
            get { return _isBikeChecked; }
            set { Set(ref _isBikeChecked, value); }
        }

        private ObservableCollection<IFavorite> _pinnedFavorites = new ObservableCollection<IFavorite>();
        public ObservableCollection<IFavorite> PinnedFavorites
        {
            get { return _pinnedFavorites; }
            set { Set(ref _pinnedFavorites, value); }
        }

        private RelayCommand _planTripCommand = null;
        public RelayCommand PlanTripCommand {
            get
            {
                if(_planTripCommand != null)
                {
                    return _planTripCommand;
                }
                _planTripCommand = new RelayCommand(PlanTrip,
                    () => (
                        (bool)IsFerryChecked || (bool)IsBusChecked || (bool)IsTramChecked 
                        || (bool)IsTrainChecked || (bool)IsMetroChecked || (bool)IsBikeChecked)
                        && FromPlace != null
                        && ToPlace != null
                    );
                this.PropertyChanged += (s, e) => _planTripCommand.RaiseCanExecuteChanged();
                return _planTripCommand;
            }
        }

        private readonly RelayCommand _planTripWideViewCommand = null;
        public RelayCommand PlanTripWideViewCommand => _planTripWideViewCommand ?? (new RelayCommand(PlanTrip));

        private readonly RelayCommand _toggleTransitPanelCommand = null;
        public RelayCommand ToggleTransitPanelCommand => _toggleTransitPanelCommand ?? new RelayCommand(TransitTogglePannel);        

        private readonly RelayCommand _setDateToTodayCommand = null;
        public RelayCommand SetDateToTodayCommand => _setDateToTodayCommand ?? new RelayCommand(SetDateToToday);

        private readonly RelayCommand<IPlace> _addFavoriteCommand = null;        
        public RelayCommand<IPlace> AddFavoriteCommand => _addFavoriteCommand ?? new RelayCommand<IPlace>(AddFavorite);

        private readonly RelayCommand<FavoritePlace> _favoritePlaceClickedCommand = null;
        public RelayCommand<FavoritePlace> FavoritePlaceClickedCommand
            => _favoritePlaceClickedCommand ?? new RelayCommand<FavoritePlace>(FavoritePlaceClicked);

        private readonly RelayCommand<IFavorite> _removePinnedFavoriteCommand = null;
        public RelayCommand<IFavorite> RemovePinnedFavoriteCommand
            => _removePinnedFavoriteCommand ?? new RelayCommand<IFavorite>(RemovePinnedFavorite);        

        public TripFormViewModel(INetworkService netService, IMessenger messengerService,
            Services.SettingsServices.SettingsService settings, IGeolocationService geolocationService,
            IDialogService dialogService)
        {
            _networkService = netService;
            _settingsService = settings;
            _messengerService = messengerService;
            _geolocationService = geolocationService;
            _dialogService = dialogService;

            _messengerService.Register<MessageTypes.FavoritesChangedMessage>(this, FavoritesChanged);
        }

        private void TransitTogglePannel()
        {            
            IsTransitPanelVisible = !IsTransitPanelVisible;
            if(IsTransitPanelVisible)
            {
                ShowHideTransitPanelText = AppResources.TripForm_FewerOptions;
                ShowHideTransitPanelGlyph = FontIconGlyphs.BoldUpArrow;
            }
            else
            {
                ShowHideTransitPanelText = AppResources.TripForm_MoreOptions;
                ShowHideTransitPanelGlyph = FontIconGlyphs.BoldDownArrow;
            }
        }

        private async void PlanTrip()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            _cts = new CancellationTokenSource();
            _cts.Token.ThrowIfCancellationRequested();

            try
            {
                SetBusy(true, AppResources.TripFrom_GettingsAddresses);

                List<IPlace> places = new List<IPlace> { FromPlace, ToPlace };
                places = await ResolvePlaces(places, _cts.Token);

                if (places.Any(x => x.Lat == default(float) || x.Lon == default(float)))
                {
                    await HandleTripFailure(places.Where(x => x.Lat == default(float) || x.Lon == default(float)).ToList());
                    SetBusy(false);
                    return;
                }
                FromPlace = places[0]; //todo: these will be replaced with some kind of list and loop when we move to "arbitrary # of legs" style input
                ToPlace = places[1]; // but for now, magic numbers wheee            

                ApiCoordinates fromCoords = new ApiCoordinates { Lat = FromPlace.Lat, Lon = FromPlace.Lon };
                ApiCoordinates toCoords = new ApiCoordinates { Lat = ToPlace.Lat, Lon = ToPlace.Lon };
                BasicTripDetails details = new BasicTripDetails(
                    fromCoords,
                    toCoords,
                    IsUsingCurrentTime ? DateTime.Now.TimeOfDay : SelectedTime,
                    SelectedDate.DateTime,
                    IsArrivalChecked == true,
                    ConstructTransitModes((bool)IsBusChecked, (bool)IsTramChecked, (bool)IsTrainChecked,
                                          (bool)IsMetroChecked, (bool)IsFerryChecked, (bool)IsBikeChecked)
                );

                SetBusy(true, AppResources.TripForm_PlanningTrip);

                var result = await _networkService.PlanTripAsync(details, _cts.Token);
                if (result.IsFailure)
                {
                    await HandleTripFailure(result);
                    SetBusy(false);
                    return;
                }

                TripPlan newPlan = new TripPlan
                {
                    ApiPlan = result.Result,
                    StartingPlaceName = FromPlace.Name,
                    EndingPlaceName = ToPlace.Name
                };
                if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
                {
                    BootStrapper.Current.SessionState.Add(NavParamKeys.PlanResults, newPlan);
                }
                else
                {
                    BootStrapper.Current.SessionState.Remove(NavParamKeys.PlanResults);
                    BootStrapper.Current.SessionState.Add(NavParamKeys.PlanResults, newPlan);
                }

                _messengerService.Send(new MessageTypes.PlanFoundMessage());
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine("Cancellation requested.");
                //Swallow otherwise. Cancellation should only happen on user request here.
            }
            finally
            {
                SetBusy(false);
            }
        }        

        /// <summary>
        /// Takes a list of places, and resolves NameOnly or UserLocation places 
        /// into a usable Place with lat/lon coordinates. Leaves other Place types alone.
        /// </summary>
        /// <param name="places">List of places to resolve.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<List<IPlace>> ResolvePlaces(List<IPlace> places, CancellationToken token)
        {
            List<Task<bool>> getAddressTasks = new List<Task<bool>>();
            for (int i = places.Count - 1; i >= 0; i--)
            {                
                int idx = i; //capturing this in the closure so it doesn't get changed out from under us in the continuation
                if (places[idx].Type == PlaceType.NameOnly)
                {
                    Task<ApiResult<GeocodingResponse>> task = _networkService.SearchAddressAsync(places[idx].Name, token);
                    getAddressTasks.Add(task.ContinueWith(resp =>
                    {
                        if (resp.Result.IsFailure)
                        {
                            return false;
                        }
                        places[idx] = new Place
                        {
                            Lon = (float)task.Result.Result.Features[0].Geometry.Coordinates[0],
                            Lat = (float)task.Result.Result.Features[0].Geometry.Coordinates[1],
                            Name = task.Result.Result.Features[0].Properties.Name,
                            Type = PlaceType.Address,
                            Id = task.Result.Result.Features[0].Properties.Id,
                            Confidence = task.Result.Result.Features[0].Properties.Confidence
                        };
                        return true;
                    }));
                }
                else if (places[idx].Type == PlaceType.UserCurrentLocation)
                {
                    Task<GenericResult<Geoposition>> task = _geolocationService.GetCurrentLocationAsync();
                    getAddressTasks.Add(task.ContinueWith(resp => {
                        if(resp.Result.IsFailure)
                        {
                            return false;
                        }
                        if(resp.Result.Result.Coordinate?.Point?.Position == null)
                        {
                            return false;
                        }
                        var loc = resp.Result.Result.Coordinate.Point.Position;
                        places[idx] = new Place
                        {
                            Lon = (float)loc.Longitude,
                            Lat = (float)loc.Latitude,
                            Name = AppResources.SuggestBoxHeader_MyLocation,
                            Type = PlaceType.UserCurrentLocation
                        };
                        return true;
                    }));
                }                
            }

            await Task.WhenAll(getAddressTasks);            
            return places;
        }

        private string ConstructTransitModes(bool isBus, bool isTram, bool isTrain, bool isMetro, bool isFerry, bool isBike)
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Constants.WalkTransitMode} ");
            if (isBike)
            {
                sb.Append($"{Constants.BikeTransitMode} ");
            }
            if (isBus)
            {
                sb.Append($"{Constants.BusTransitMode} ");
            }
            if (isFerry)
            {
                sb.Append($"{Constants.FerryTransitMode} ");
            }
            if (isTrain)
            {
                sb.Append($"{Constants.TrainTransitMode} ");
            }
            if (isMetro)
            {
                sb.Append($"{Constants.MetroTransitMode} ");
            }
            if (isTram)
            {
                sb.Append($"{Constants.TramTransitMode} ");
            }
            return sb.ToString().Trim().Replace(" ", ",");
        }        

        private void SetDateToToday()
        {
            SelectedDate = DateTime.Now;
        }        

        private void AddFavorite(IPlace place)
        {
            FavoritePlace newFavoritePlace = new FavoritePlace
            {
                FontIconGlyph = FontIconGlyphs.FilledStar,
                IconFontFace = "Segoe MDL2 Assets",
                Lat = place.Lat,
                Lon = place.Lon,
                Name = place.Name,
                Type = PlaceType.FavoritePlace,
                UserChosenName = place.Name
            };
            _settingsService.AddFavorite(newFavoritePlace);
        }

        private void FillPinnedFavorites()
        {                        
            if(_settingsService.PinnedFavorites.Count == 0)
            {
                PinnedFavorites.Clear();
                return;
            }            

            int numPinnedToDisplay = _settingsService.PinnedFavoritesDisplayNumber;
            var pinned = _settingsService.PinnedFavorites.Count >= numPinnedToDisplay 
                ? new List<IFavorite>(_settingsService.PinnedFavorites.Take(numPinnedToDisplay)) 
                : new List<IFavorite>(_settingsService.PinnedFavorites);

            var toRemove = PinnedFavorites.Except(pinned).ToList();
            foreach(var staleFave in toRemove)
            {
                PinnedFavorites.Remove(staleFave);
            }
            foreach (var newFace in pinned.Except(PinnedFavorites))
            {
                PinnedFavorites.Add(newFace);
            }            
        }

        private void FavoritesChanged(MessageTypes.FavoritesChangedMessage obj)
        {
            if (obj.AddedFavorites?.Count > 0
                && _settingsService.PinnedFavorites?.Count < _settingsService.PinnedFavoritesDisplayNumber)
            {
                int maxToAdd = _settingsService.PinnedFavoritesDisplayNumber - _settingsService.PinnedFavorites.Count;
                int toAdd = Math.Min(maxToAdd, obj.AddedFavorites.Count);
                foreach(var newPinned in obj.AddedFavorites.Take(toAdd))
                {
                    _settingsService.AddPinnedFavorite(newPinned);
                }
            }
            if (obj.RemovedFavorites?.Count > 0 && _settingsService.PinnedFavorites?.Count > 0)
            {
                foreach (var removed in obj.RemovedFavorites)
                {
                    _settingsService.RemovePinnedFavorite(removed);
                }
            }
            if(obj.RemovedFavorites?.Count > 0 || obj.AddedFavorites?.Count > 0)
            {
                FillPinnedFavorites();
            }
        }

        private void FavoritePlaceClicked(FavoritePlace obj)
        {
            ToPlace = obj;
            if (PlanTripCommand.CanExecute(null))
            {
                PlanTrip();
            }
        }

        private void RemovePinnedFavorite(IFavorite favorite)
        {
            _settingsService.RemovePinnedFavorite(favorite);
            FillPinnedFavorites();
        }

        private async Task HandleTripFailure(ApiResult<ApiPlan> result)
        {
            string errorMessage = null;
            if (!String.IsNullOrEmpty(result.Failure.FriendlyError))
            {
                errorMessage = result.Failure.FriendlyError;
            }
            else if (result.Failure.Reason == FailureReason.NoResults)
            {
                errorMessage = AppResources.DialogMessage_NoTripsFoundNoResults;
            }
            else if (result.Failure.Reason == FailureReason.InternalServerError
                     || result.Failure.Reason == FailureReason.NoConnection
                     || result.Failure.Reason == FailureReason.ServerDown)
            {
                errorMessage = AppResources.DialogMessage_NoTripsFoundNoServer;
            }
            else if(result.Failure.Reason == FailureReason.Canceled)
            {
                return; //swallow cancellation, the only way it can happen here is user-triggered
            }
            else
            {
                errorMessage = AppResources.DialogMessage_NoTripsFoundUnknown;
            }
            await _dialogService.ShowDialog(errorMessage, AppResources.DialogTitle_NoTripsFound);
        }        

        private async Task HandleTripFailure(IList<IPlace> resolutionFailures)
        {
            if (resolutionFailures.Any(x => x.Type == PlaceType.UserCurrentLocation))
            {
                await _dialogService.ShowDialog(AppResources.DialogMessage_UserLocationFailed, AppResources.DialogTitle_NoLocationFound);
                return;
            }

            StringBuilder error = new StringBuilder();
            error.AppendLine(AppResources.DialogMessage_PlaceResolutionFailed);
            foreach (var place in resolutionFailures)
            {
                error.AppendLine($"● {place.Name}");
            }
            await _dialogService.ShowDialog(error.ToString(), AppResources.DialogTitle_NoLocationFound);
        }

        private void SetBusy(bool newBusy, string message = null)
        {
            if(newBusy == _isBusy && message == _currentBusyMessage)
            {
                return;
            }
            else
            {
                _isBusy = newBusy;
                _currentBusyMessage = message;
                Views.Busy.SetBusy(newBusy, true, message);                
            }
        }        

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            FillPinnedFavorites();
            BootStrapper.BackRequested += BootStrapper_BackRequested;
            await Task.CompletedTask;
        }

        private void BootStrapper_BackRequested(object sender, HandledEventArgs e)
        {
            if (_isBusy)
            {
                //don't mark this as handled, because the Busy control will do that when it dismisses itself.                
                _messengerService.Send(new MessageTypes.NavigationCanceled());                
                _cts.Cancel();                
            }
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {            
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            BootStrapper.BackRequested -= BootStrapper_BackRequested;
            await Task.CompletedTask;
        }
    }
}
