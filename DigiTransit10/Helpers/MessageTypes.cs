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
