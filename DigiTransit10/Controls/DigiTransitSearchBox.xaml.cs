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

namespace DigiTransit10.Controls
{
    public sealed partial class DigiTransitSearchBox : UserControl, INotifyPropertyChanged
    {
        private readonly INetworkService _networkService;
        private CancellationTokenSource _currentToken = new CancellationTokenSource();
        private readonly DispatcherTimer _textChangeThrottle;        

        private readonly GroupedPlaceList _stopList = new GroupedPlaceList(ModelEnums.PlaceType.Stop, 
            AppResources.SuggestBoxHeader_TransitStops);

        private readonly GroupedPlaceList _addressList = new GroupedPlaceList(ModelEnums.PlaceType.Address, 
            AppResources.SuggestBoxHeader_Addresses);

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

        public DigiTransitSearchBox()
        {
            this.InitializeComponent();
            if(Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            SuggestedPlaces.Add(_stopList);
            SuggestedPlaces.Add(_addressList);
            PlacesCollection.Source = SuggestedPlaces;

            _networkService = ServiceLocator.Current.GetInstance<INetworkService>();
            _textChangeThrottle = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _textChangeThrottle.Tick += SearchTick;
        }

        public static readonly DependencyProperty SelectedPlaceProperty =
            DependencyProperty.Register("SelectedPlace", typeof(Place), typeof(DigiTransitSearchBox), new PropertyMetadata(null, 
                (obj, args) => {
                    DigiTransitSearchBox box = obj as DigiTransitSearchBox;
                    if(box == null)
                    {
                        return;
                    }

                    Place newPlace = args.NewValue as Place;
                    if(newPlace == null)
                    {
                        return;
                    }

                    box.SearchText = newPlace.Name;
                }));
        public Place SelectedPlace
        {
            get { return (Place)GetValue(SelectedPlaceProperty); }
            set { SetValue(SelectedPlaceProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(DigiTransitSearchBox), new PropertyMetadata(null));
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

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(DigiTransitSearchBox), new PropertyMetadata(null));
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }                

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(String.IsNullOrWhiteSpace(SearchBox.Text))
            {                
                _stopList.Clear();
                _addressList.Clear();
                // this has to happen after the list clearnig. clearing _stopList seems to force a SuggestionChosen(), which grabs the first item in the still-filled _addressList.
                SearchText = ""; 
                SelectedPlace = null;
                return;
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                SelectedPlace = null;
                _textChangeThrottle.Stop();
                _textChangeThrottle.Start();
            }
            if(!args.CheckCurrent())
            {
                SearchText = SearchBox.Text;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //cancel any outstanding Searches, set text field to the Place chosen
            IsWaiting = false;

            _currentToken.Cancel();

            SelectedPlace = (Place)args.SelectedItem;
            SearchText = SelectedPlace.Name;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //if the ChosenSuggestion property is not null, set textbox text (may happen automatically)
            //fire a "search" event that a viewmodel can listen for
            IsWaiting = false;

            if(args.ChosenSuggestion != null)
            {                
                SelectedPlace = (Place)args.ChosenSuggestion;

                if(Command != null && Command.CanExecute(SelectedPlace))
                {
                    Command.Execute(SelectedPlace);
                }
            }
            else
            {
                if(Command != null && Command.CanExecute(args.QueryText))
                {
                    Command.Execute(args.QueryText);
                }
            }
        }

        private async void SearchTick(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("Firing SearchTick!");
            _textChangeThrottle.Stop();
            await TriggerSearch(this.SearchBox.Text);
        }

        private async Task TriggerSearch(string searchString)
        {
            if(_currentToken != null && !_currentToken.IsCancellationRequested)
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
            GeocodingResponse result;
            try
            {
                result = await _networkService.SearchAddress(searchString, token);
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine("SearchAddresses operation cancelled.");
                return;
            }
            if (result == null || result.Features.Length < 1)
            {                
                return;
            }

            //Remove entries in old list not in new response
            List<string> responseIds = result.Features.Select(x => x.Properties.Id).ToList();
            List<Place> stalePlaces = _addressList.Where(x => !responseIds.Contains(x.Id)).ToList();
            foreach (var stale in stalePlaces)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => _addressList.Remove(stale));
            }

            foreach (var place in result.Features)
            {
                if (_addressList.Any(x => x.Id == place.Properties.Id))
                {
                    continue;
                }
                string name = place.Properties.Name;
                if (place.Properties.Street != null) name += $", {place.Properties.Street}";
                if (place.Properties.HouseNumber != null) name += $" {place.Properties.HouseNumber}";                
                Place foundPlace = new Place
                {
                    Id= place.Properties.Id,
                    Name = name,
                    Lat = (float)place.Geometry.Coordinates[1],
                    Lon = (float)place.Geometry.Coordinates[0],
                    Type = ModelEnums.PlaceType.Address,
                    Confidence = place.Properties.Confidence
                };
                DispatcherHelper.CheckBeginInvokeOnUI(() => _addressList.Add(foundPlace));
            }            
        }

        private async Task SearchStops(string searchString, CancellationToken token)
        {
            List<ApiStop> result;
            try
            {
                result = await _networkService.GetStops(searchString, token);
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine("SearchStops operation cancelled.");
                return;
            }
            if(result == null || result.Count < 1)
            {
                return;
            }

            //Remove entries in old list not in new response
            List<string> responseIds = result.Select(x => x.Id).ToList();
            List<Place> stalePlaces = _stopList.Where(x => !responseIds.Contains(x.Id)).ToList();
            foreach (var stale in stalePlaces)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>_stopList.Remove(stale));
            }

            foreach(var stop in result)
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
                DispatcherHelper.CheckBeginInvokeOnUI(() =>_stopList.AddSorted(foundPlace));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
