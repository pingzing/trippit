using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using DigiTransit10.Services;
using DigiTransit10.Localization.Strings;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Models.Geocoding;
using GalaSoft.MvvmLight.Threading;
using DigiTransit10.Services.SettingsServices;
using DigiTransit10.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace DigiTransit10.Controls
{
    public sealed partial class DigiTransitSearchBox : UserControl, INotifyPropertyChanged
    {
        private static readonly IPlaceComparer _placeComparer = new IPlaceComparer();
        private static readonly SettingsService _settingsService = null;
        private static readonly INetworkService _networkService = null;
        private static readonly IMessenger _messengerService = null;

        private CancellationTokenSource _currentToken = new CancellationTokenSource();        

        private readonly GroupedPlaceList _stopList = new GroupedPlaceList(ModelEnums.PlaceType.Stop,
            AppResources.SuggestBoxHeader_TransitStops);

        private readonly GroupedPlaceList _addressList = new GroupedPlaceList(ModelEnums.PlaceType.Address,
            AppResources.SuggestBoxHeader_Addresses);

        private readonly GroupedPlaceList _userCurrentLocationList = new GroupedPlaceList(ModelEnums.PlaceType.UserCurrentLocation,
            AppResources.SuggestBoxHeader_MyLocation);

        private readonly GroupedPlaceList _favoritePlacesList = new GroupedPlaceList(ModelEnums.PlaceType.FavoritePlace,
            AppResources.SuggestBoxHeader_FavoritePlaces);

        private ObservableCollection<GroupedPlaceList> _suggestedPlaces = new ObservableCollection<GroupedPlaceList>();
        public ObservableCollection<GroupedPlaceList> SuggestedPlaces
        {
            get { return _suggestedPlaces; }
            set
            {
                if (_suggestedPlaces != value)
                {
                    _suggestedPlaces = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isWaiting = false;
        public bool IsWaiting
        {
            get { return _isWaiting; }
            set
            {
                if (_isWaiting != value)
                {
                    _isWaiting = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsFavoriteButtonEnabled
        {
            get
            {
                if(SelectedPlace == null)
                {
                    return false;
                }                
                if(SelectedPlace.Type == ModelEnums.PlaceType.NameOnly
                    || SelectedPlace.Type == ModelEnums.PlaceType.UserCurrentLocation
                    || SelectedPlace.Type == ModelEnums.PlaceType.FavoritePlace
                    || SelectedPlace.Lat == default(float) 
                    || SelectedPlace.Lon == default(float))
                {
                    return false;
                }
                return true;
            }
        }

        public string FavoriteButtonGlyph
        {
            get
            {
                if (SelectedPlace == null)
                {
                    return FontIconGlyphs.HollowStar;
                }
                if(SelectedPlace.Type == ModelEnums.PlaceType.NameOnly
                    || SelectedPlace.Type == ModelEnums.PlaceType.UserCurrentLocation
                    || SelectedPlace.Lat == default(float)
                    || SelectedPlace.Lon == default(float))
                {
                    return FontIconGlyphs.HollowStar;
                }
                if(SelectedPlace.Type == ModelEnums.PlaceType.FavoritePlace 
                    || _settingsService.Favorites.Where(x => x is FavoritePlace)
                        .Any(x => ((FavoritePlace)x).Lat == SelectedPlace.Lat 
                            && ((FavoritePlace)x).Lon == SelectedPlace.Lon))
                {
                    return FontIconGlyphs.FilledStar;
                }
                else
                {
                    return FontIconGlyphs.HollowStar;
                }
            }
        }

        static DigiTransitSearchBox()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }
            _networkService = ServiceLocator.Current.GetInstance<INetworkService>();            
            _settingsService = ServiceLocator.Current.GetInstance<SettingsService>();
            _messengerService = ServiceLocator.Current.GetInstance<IMessenger>();
        }

        public DigiTransitSearchBox()
        {
            this.InitializeComponent();
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }                                    

            _userCurrentLocationList.Add(new Place { Name = AppResources.SuggestBoxHeader_MyLocation, Type = ModelEnums.PlaceType.UserCurrentLocation });
            foreach(var favorite in _settingsService.Favorites.Where(x => x is FavoritePlace))
            {
                _favoritePlacesList.AddSorted(favorite as FavoritePlace);
            }
            SuggestedPlaces.Add(_userCurrentLocationList);
            SuggestedPlaces.Add(_favoritePlacesList);
            SuggestedPlaces.Add(_stopList);
            SuggestedPlaces.Add(_addressList);
            PlacesCollection.Source = SuggestedPlaces;

            this.SearchBox.GotFocus += SearchBox_GotFocus;
            _messengerService.Register<MessageTypes.FavoritesChangedMessage>(this, FavoritesChanged);
        }

        public static readonly DependencyProperty SelectedPlaceProperty =
            DependencyProperty.Register("SelectedPlace", typeof(IPlace), typeof(DigiTransitSearchBox), new PropertyMetadata(null,
                (obj, args) =>
                {
                    DigiTransitSearchBox box = obj as DigiTransitSearchBox;
                    if (box == null)
                    {
                        return;
                    }

                    IPlace newPlace = args.NewValue as IPlace;
                    if (newPlace != null)
                    {
                        box.SearchText = newPlace.Name;
                    }
                    else
                    {
                        box.SearchText = "";
                    }
                    
                    box.RaisePropertyChanged(nameof(IsFavoriteButtonEnabled));
                    box.RaisePropertyChanged(nameof(FavoriteButtonGlyph));
                }));
        public IPlace SelectedPlace
        {
            get { return (IPlace)GetValue(SelectedPlaceProperty); }
            set { SetValue(SelectedPlaceProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(DigiTransitSearchBox), new PropertyMetadata(""));
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
        
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DigiTransitSearchBox), new PropertyMetadata(null));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty FavoriteTappedCommandProperty =
            DependencyProperty.Register("FavoriteTappedCommand", typeof(ICommand), typeof(DigiTransitSearchBox), new PropertyMetadata(null));
        public ICommand FavoriteTappedCommand
        {
            get { return (ICommand)GetValue(FavoriteTappedCommandProperty); }
            set { SetValue(FavoriteTappedCommandProperty, value); }
        }                

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(DigiTransitSearchBox), new PropertyMetadata(null));
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (String.IsNullOrWhiteSpace(SearchBox.Text))
            {
                _stopList.Clear();
                _addressList.Clear();                

                // this has to happen after the list clearing. clearing _stopList seems to force a SuggestionChosen(), which grabs the first item in the still-filled _addressList.
                SearchText = "";
                SelectedPlace = null;
                return;
            }

            if (!args.CheckCurrent())
            {
                SearchText = SearchBox.Text;
            }

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                SelectedPlace = new Place
                {
                    Name = SearchText,
                    Type = ModelEnums.PlaceType.NameOnly
                };
                await TriggerSearch(SearchText);
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //cancel any outstanding Searches, set text field to the Place chosen
            IsWaiting = false;
            _currentToken?.Cancel();

            SelectedPlace = (IPlace)args.SelectedItem;
            SearchText = SelectedPlace.Name;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {                        
            //todo: should this also be an actual event, as well as the Command?
            IsWaiting = false;
            _currentToken?.Cancel();

            if (args.ChosenSuggestion != null)
            {
                SelectedPlace = (IPlace)args.ChosenSuggestion;
                SearchText = SelectedPlace.Name;

                if (Command != null && Command.CanExecute(SelectedPlace))
                {
                    Command.Execute(SelectedPlace);
                }
            }
            else
            {
                SelectedPlace = new Place
                {
                    Confidence = null,
                    Id = null,
                    Lat = 0,
                    Lon = 0,
                    Name = args.QueryText,
                    Type = ModelEnums.PlaceType.NameOnly
                };                
                SearchText = args.QueryText;

                if(SearchBox.IsSuggestionListOpen)
                {
                    SearchBox.IsSuggestionListOpen = false;
                }

                if (Command != null && Command.CanExecute(args.QueryText))
                {
                    Command.Execute(args.QueryText);
                }
            }
        }        

        private async Task TriggerSearch(string searchString)
        {
            if (_currentToken != null && !_currentToken.IsCancellationRequested)
            {
                _currentToken.Cancel();
            }

            _currentToken = new CancellationTokenSource();

            Task addressTask = SearchAddresses(searchString, _currentToken.Token);
            Task stopTask = SearchStops(searchString, _currentToken.Token);

            IsWaiting = true;
            try
            {
                await Task.WhenAll(addressTask, stopTask);
            }
            catch (OperationCanceledException)
            {
                IsWaiting = false;
                return;
            }
            IsWaiting = false;
        }

        private async Task SearchAddresses(string searchString, CancellationToken token)
        {
            //not sending the token into the network request, because they get called super frequently, and cancelling a network request is a HUGE perf hit on phones
            var response = await _networkService.SearchAddressAsync(searchString);
            if (token.IsCancellationRequested || response.IsFailure)
            {
                return;
            }

            GeocodingResponse result = response.Result;

            //Remove entries in old list not in new response
            List<string> responseIds = result.Features.Select(x => x.Properties.Id).ToList();
            List<IPlace> stalePlaces = _addressList.Where(x => !responseIds.Contains(x.Id)).ToList();
            foreach (var stale in stalePlaces)
            {
                _addressList.Remove(stale);
            }

            foreach (var place in result.Features)
            {
                if (_addressList.Any(x => x.Id == place.Properties.Id))
                {
                    IPlace address = _addressList.First(x => x.Id == place.Properties.Id);
                    if (address.Confidence != place.Properties.Confidence)
                    {
                        address.Confidence = place.Properties.Confidence;
                    }
                    continue;
                }
                string name = place.Properties.Name;                
                Place foundPlace = new Place
                {
                    Id = place.Properties.Id,
                    Name = name,
                    Lat = (float)place.Geometry.Coordinates[1],
                    Lon = (float)place.Geometry.Coordinates[0],
                    Type = ModelEnums.PlaceType.Address,
                    Confidence = place.Properties.Confidence
                };
                _addressList.AddSorted(foundPlace, _placeComparer);                                
            }
            _addressList.SortInPlace(x => x, _placeComparer);
        }

        private async Task SearchStops(string searchString, CancellationToken token)
        {
            //not sending the token into the network request, because they get called super frequently, and cancelling a network request is a HUGE perf hit on phones
            var response = await _networkService.GetStopsAsync(searchString);
            if (token.IsCancellationRequested || response.IsFailure)
            {
                return;
            }

            var result = response.Result;

            //Remove entries in old list not in new response
            List<string> responseIds = result.Select(x => x.Id).ToList();
            List<IPlace> stalePlaces = _stopList.Where(x => !responseIds.Contains(x.Id)).ToList();
            foreach (var stale in stalePlaces)
            {
                _stopList.Remove(stale);
            }

            foreach (var stop in result)
            {
                if (_stopList.Any(x => x.Id == stop.Id))
                {
                    continue;
                }
                Place foundPlace = new Place
                {
                    Id = stop.Id,
                    Name = $"{stop.Name}, {stop.Code}",
                    Lat = stop.Lat,
                    Lon = stop.Lon,
                    Type = ModelEnums.PlaceType.Stop
                };
                _stopList.AddSorted(foundPlace, _placeComparer);                
            }
            _stopList.SortInPlace(x => x, _placeComparer);
        }

        private void FavoritesChanged(MessageTypes.FavoritesChangedMessage obj)
        {
            if(obj.AddedFavorites?.Count > 0)
            {
                foreach(var added in obj.AddedFavorites)
                {
                    IPlace castAdded = (IPlace)added;
                    _favoritePlacesList.AddSorted(castAdded);
                }
            }
            if(obj.RemovedFavorites?.Count > 0)
            {
                foreach(var removed in obj.RemovedFavorites)
                {
                    IPlace castRemoved = (IPlace)removed;
                    _favoritePlacesList.Remove(castRemoved);
                }
            }

            RaisePropertyChanged(nameof(IsFavoriteButtonEnabled));
            RaisePropertyChanged(nameof(FavoriteButtonGlyph));
        }

        private void AddToFavoriteButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
           if(FavoriteTappedCommand != null && FavoriteTappedCommand.CanExecute(SelectedPlace))
            {
                FavoriteTappedCommand.Execute(SelectedPlace);
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!SearchBox.IsSuggestionListOpen)
            {
                SearchBox.IsSuggestionListOpen = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
