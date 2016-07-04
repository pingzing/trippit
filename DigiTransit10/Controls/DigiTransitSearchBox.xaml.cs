using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using DigiTransit10.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.Controls
{
    public sealed partial class DigiTransitSearchBox : UserControl, INotifyPropertyChanged
    {
        private readonly INetworkService _networkService;
        private CancellationTokenSource _currentToken = new CancellationTokenSource();
        private DispatcherTimer _textChangeThrottle;

        private ObservableCollection<Place> _suggestedPlaces = new ObservableCollection<Place>();
        public ObservableCollection<Place> SuggestedPlaces
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
            _networkService = ServiceLocator.Current.GetInstance<INetworkService>();
            _textChangeThrottle = new DispatcherTimer();
            _textChangeThrottle.Interval = TimeSpan.FromMilliseconds(250);
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
                SearchText = "";
                SuggestedPlaces.Clear();
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

        private void SearchTick(object sender, object e)
        {
            _textChangeThrottle.Stop();
            TriggerSearch(this.SearchBox.Text).DoNotAwait();
        }

        private async Task TriggerSearch(string searchString)
        {
            if(!_currentToken.IsCancellationRequested)
            {
                _currentToken.Cancel();
            }

            _currentToken = new CancellationTokenSource();
            SuggestedPlaces.Clear(); //remove this and do a per-call removal in each task below

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
            var result = await _networkService.SearchAddress(searchString, token);
            if (result == null || result.Features.Length < 1)
            {                
                return;
            }
            foreach (var place in result.Features)
            {
                string name = place.Properties.Name;
                if (place.Properties.Street != null) name += $", {place.Properties.Street}";
                if (place.Properties.HouseNumber != null) name += $" {place.Properties.HouseNumber}";
                name += $", C:{place.Properties.Confidence}";
                Place foundPlace = new Place
                {
                    Name = name,
                    Lat = (float)place.Geometry.Coordinates[1],
                    Lon = (float)place.Geometry.Coordinates[0],
                    Type = ModelEnums.PlaceType.Address,
                    Confidence = place.Properties.Confidence
                };
                SuggestedPlaces.AddSorted(foundPlace);
            }
        }

        private async Task SearchStops(string searchString, CancellationToken token)
        {
            var result = await _networkService.GetStops(searchString, token);
            if(result == null || result.Count < 1)
            {
                return;
            }
            foreach(var stop in result)
            {
                Place foundPlace = new Place
                {
                    Name = $"{stop.Name}, {stop.Code}",
                    Lat = stop.Lat,
                    Lon = stop.Lon,
                    Type = ModelEnums.PlaceType.Stop                    
                };
                SuggestedPlaces.AddSorted(foundPlace);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
