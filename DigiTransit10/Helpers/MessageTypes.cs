using DigiTransit10.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.Helpers
{
    public static class MessageTypes
    {
        public class PlanFoundMessage { }
        public class CenterAroundFavoritesOnMap { }

        public class FavoritesChangedMessage
        {
            public ReadOnlyCollection<IFavorite> AddedFavorites { get; }
            public ReadOnlyCollection<IFavorite> RemovedFavorites { get; }

            public FavoritesChangedMessage(IList<IFavorite> added, IList<IFavorite> removed)
            {
                if (added != null) AddedFavorites = new ReadOnlyCollection<IFavorite>(added);
                if (removed != null) RemovedFavorites = new ReadOnlyCollection<IFavorite>(removed);
            }
        }

        public class ViewPlanDetails
        {
            public TripItinerary BackingModel { get; private set; }

            public ViewPlanDetails(TripItinerary model)
            {
                BackingModel = model;
            }
        }

        public class ViewPlanStrips { }

        public class NavigationCanceled { }
    }
}
