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

namespace DigiTransit10.ViewModels
{
    public class TripFormViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly Services.SettingsServices.SettingsService _settingsService;
        private readonly IMessenger _messengerService;
        private readonly IGeolocationService _geolocationService;

        private CancellationTokenSource _cts = new CancellationTokenSource();

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

        private Place _fromPlace;
        public Place FromPlace
        {
            get { return _fromPlace; }
            set { Set(ref _fromPlace, value); }
        }

        private Place _toPlace;
        public Place ToPlace
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

        private readonly RelayCommand _planTripNarrowViewCommand = null;
        public RelayCommand PlanTripNarrowViewCommand => _planTripNarrowViewCommand ?? (new RelayCommand(PlanTripNarrowView));

        private readonly RelayCommand _planTripWideViewCommand = null;
        public RelayCommand PlanTripWideViewCommand => _planTripWideViewCommand ?? (new RelayCommand(PlanTripWideView));

        private readonly RelayCommand _toggleTransitPanelCommand = null;
        public RelayCommand ToggleTransitPanelCommand => _toggleTransitPanelCommand ?? new RelayCommand(TransitTogglePannel);

        public TripFormViewModel(INetworkService netService, IMessenger messengerService,
            Services.SettingsServices.SettingsService settings, 
            IGeolocationService geolocationService)
        {
            _networkService = netService;
            _settingsService = settings;
            _messengerService = messengerService;
            _geolocationService = geolocationService;
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

        private async Task PlanTrip()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            _cts = new CancellationTokenSource();

            Views.Busy.SetBusy(true, AppResources.TripFrom_GettingsAddresses);

            List<Place> places = new List<Place>();
            places.Add(FromPlace);
            places.Add(ToPlace);

            places = await ResolvePlaces(places, _cts.Token);
            FromPlace = places[0]; //todo: these will be replaced with some kind of list and loop when we move to "arbitrary # of legs" style input
            ToPlace = places[1]; // but for now, magic numbers wheee

            ApiCoordinates fromCoords = new ApiCoordinates { Lat = FromPlace.Lat, Lon = FromPlace.Lon };
            ApiCoordinates toCoords = new ApiCoordinates { Lat = ToPlace.Lat, Lon = ToPlace.Lon };            
            BasicTripDetails details = new BasicTripDetails(
                fromCoords,
                toCoords,
                SelectedTime,
                SelectedDate.DateTime,
                IsArrivalChecked == true,
                ConstructTransitModes((bool)IsBusChecked, (bool)IsTramChecked, (bool)IsTrainChecked,
                                      (bool)IsMetroChecked, (bool)IsFerryChecked, (bool)IsBikeChecked)
            );

            Views.Busy.SetBusy(true, AppResources.TripForm_PlanningTrip);

            var result = await _networkService.PlanTrip(details);
            if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                BootStrapper.Current.SessionState.Add(NavParamKeys.PlanResults, result);
            }
            else
            {
                BootStrapper.Current.SessionState.Remove(NavParamKeys.PlanResults);
                BootStrapper.Current.SessionState.Add(NavParamKeys.PlanResults, result);
            }
            Views.Busy.SetBusy(false);

        }        

        private async Task<List<Place>> ResolvePlaces(List<Place> places, CancellationToken token)
        {
            List<Task<bool>> getAddressTasks = new List<Task<bool>>();
            for (int i = places.Count - 1; i >= 0; i--)
            {                
                int idx = i; //capturing this in the closure so it doesn't get changed out from under us in the continuation
                if (places[idx].Type == ModelEnums.PlaceType.NameOnly)
                {
                    Task<GeocodingResponse> task = _networkService.SearchAddress(places[idx].Name, token);
                    getAddressTasks.Add(task.ContinueWith(resp =>
                    {
                        if (resp.Result == null || resp.Result.Features.Length == 0)
                        {
                            return false;
                        }
                        places[idx] = new Place
                        {
                            Lon = (float)task.Result.Features[0].Geometry.Coordinates[0],
                            Lat = (float)task.Result.Features[0].Geometry.Coordinates[1],
                            Name = task.Result.Features[0].Properties.Name,
                            Type = ModelEnums.PlaceType.Address,
                            Id = task.Result.Features[0].Properties.Id,
                            Confidence = task.Result.Features[0].Properties.Confidence
                        };
                        return true;
                    }));
                }
                else if (places[idx].Type == ModelEnums.PlaceType.UserCurrentLocation)
                {
                    Task<Geoposition> task = _geolocationService.GetCurrentLocation();
                    getAddressTasks.Add(task.ContinueWith(resp => {
                        if(resp == null || resp.Result?.Coordinate?.Point?.Position == null)
                        {
                            return false;
                        }
                        var loc = resp.Result.Coordinate.Point.Position;
                        places[idx] = new Place
                        {
                            Lon = (float)loc.Longitude,
                            Lat = (float)loc.Latitude,
                            Name = AppResources.SuggestBoxHeader_MyLocation,
                            Type = ModelEnums.PlaceType.UserCurrentLocation
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

        private async void PlanTripNarrowView()
        {
            await PlanTrip();
            _messengerService.Send<string>(Constants.NarrowKey, MessageTypes.PlanFoundMessage);
        }

        private async void PlanTripWideView()
        {
            await PlanTrip();
            _messengerService.Send<string>(Constants.WideKey, MessageTypes.PlanFoundMessage);
        }
    }
}
