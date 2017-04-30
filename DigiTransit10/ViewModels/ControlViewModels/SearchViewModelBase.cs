using DigiTransit10.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System;
using System.Collections.Generic;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    /// <summary>
    /// A common interface for all the ViewModels that get used on the SearchPage.
    /// </summary>
    public interface ISearchViewModel
    {
        CancellationTokenSource TokenSource { get; }
        SearchSection OwnedBy { get; }
        string Title { get; set; }
        bool IsOverviewLoading { get; set; }
        ObservableCollection<IMapPoi> MapPlaces { get; set; }        
        ObservableCollection<ColoredMapLine> MapLines { get; set; }        
        ObservableCollection<ColoredGeocircle> MapCircles { get; set; }
        void SetMapSelectedPlace(IEnumerable<Guid> obj);
    }
}
