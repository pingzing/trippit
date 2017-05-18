using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services;
using DigiTransit10.Styles;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class LineSearchContentViewModel : BindableBase, ISearchViewModel
    {        
        private readonly INetworkService _networkService;
        private readonly IMessenger _messenger;

        private CancellationTokenSource _tokenSource = null;
        public CancellationTokenSource TokenSource => _tokenSource;

        public SearchSection OwnedBy { get; private set; }

        private ObservableCollection<LineSearchElementViewModel> _linesResultList = new ObservableCollection<LineSearchElementViewModel>();
        public ObservableCollection<LineSearchElementViewModel> LinesResultList
        {
            get { return _linesResultList; }
            set { Set(ref _linesResultList, value); }
        }

        private ObservableCollection<IMapPoi> _mapPlaces = new ObservableCollection<IMapPoi>();
        public ObservableCollection<IMapPoi> MapPlaces
        {
            get { return _mapPlaces; }
            set { Set(ref _mapPlaces, value); }
        }

        private ObservableCollection<ColoredMapLine> _mapLines = new ObservableCollection<ColoredMapLine>();
        public ObservableCollection<ColoredMapLine> MapLines
        {
            get { return _mapLines; }
            set { Set(ref _mapLines, value); }
        }

        private ObservableCollection<ColoredGeocircle> _mapCircles = new ObservableCollection<ColoredGeocircle>();
        public ObservableCollection<ColoredGeocircle> MapCircles
        {
            get { return _mapCircles; }
            set { Set(ref _mapCircles, value); }
        }

        private string _linesSearchBoxText;
        public string LinesSearchBoxText
        {
            get { return _linesSearchBoxText; }
            set { Set(ref _linesSearchBoxText, value); }
        }

        private LineSearchElementViewModel _selectedLine;
        public LineSearchElementViewModel SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                Set(ref _selectedLine, value);
                UpdateSelectedLine(value);
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private bool _isOverviewLoading;
        public bool IsOverviewLoading
        {
            get { return _isOverviewLoading; }
            set { Set(ref _isOverviewLoading, value); }
        }

        private RelayCommand<string> _getLinesCommand;
        public RelayCommand<string> GetLinesCommand => _getLinesCommand ?? (_getLinesCommand = new RelayCommand<string>(GetLinesAsync));        

        public LineSearchContentViewModel(INetworkService network, IMessenger messenger, SearchSection ownedBy, string title)
        {            
            _networkService = network;
            _messenger = messenger;
            OwnedBy = ownedBy;
            Title = title;
        }

        public async void GetLinesAsync(string searchText)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return;
            }

            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
            _tokenSource = new CancellationTokenSource();

            ApiResult<IEnumerable<TransitLine>> response = await _networkService.GetLinesAsync(searchText, _tokenSource.Token);
            if (response.IsFailure || _tokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            // TODO: Merge these in instead of nuking and recreating, maybe? low prio
            LinesResultList = new ObservableCollection<LineSearchElementViewModel>(response.Result.Select(x => new LineSearchElementViewModel { BackingLine = x }));
        }

        // To be used by other parts of the app wanting to display info for a given line
        public async Task GetLinesByIdAsync(string gtfsId)
        {
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
            _tokenSource = new CancellationTokenSource();

            ApiResult<IEnumerable<TransitLine>> response = await _networkService.GetLinesAsync(new string[] { gtfsId }, _tokenSource.Token);
            if (response.IsFailure || _tokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            LinesResultList = new ObservableCollection<LineSearchElementViewModel>(response.Result.Select(x => new LineSearchElementViewModel { BackingLine = x }));
            SelectedLine = LinesResultList.FirstOrDefault();
        }

        private void UpdateSelectedLine(LineSearchElementViewModel element)
        {
            MapLines.Clear();
            MapPlaces.Clear();

            if (element == null)
            {
                return;
            }

            List<ColoredMapLinePoint> linePoints = element
                .BackingLine
                .Points
                .Select(x => new ColoredMapLinePoint(
                                    BasicGeopositionExtensions.Create(0.0, x.Longitude, x.Latitude),
                                    HslColors.GetModeColor(element.BackingLine.TransitMode)))
                .ToList();

            var mapLine = new ColoredMapLine(linePoints);
            MapLines.Clear();
            MapLines.AddRange(new List<ColoredMapLine> { mapLine });

            List<IMapPoi> stops = new List<IMapPoi>();
            foreach (var stop in element.BackingLine.Stops)
            {
                stops.Add(new BasicMapPoi { Coords = stop.Coords, Name = stop.Name });
            }            
            MapPlaces.AddRange(stops);

            _messenger.Send(new MessageTypes.SearchLineSelectionChanged());
        }

        public void SetMapSelectedPlace(IEnumerable<Guid> obj)
        {
            // TODO: No functionality here yet. Not quite sure what to do with it.
        }
    }
}
