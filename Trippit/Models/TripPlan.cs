using System.Collections.Generic;
using System.Linq;
using Trippit.Models.ApiModels;

namespace Trippit.Models
{
    public class TripPlan
    {
        public string StartingPlaceName { get; set; }
        public string EndingPlaceName { get; set; }
        public List<TripItinerary> PlanItineraries { get; set; }

        public TripPlan(ApiPlan apiPlan, string startingPlaceName, string endingPlaceName)
        {
            StartingPlaceName = startingPlaceName;
            EndingPlaceName = endingPlaceName;

            PlanItineraries = apiPlan.Itineraries
                .Select(x => new TripItinerary(x, StartingPlaceName, EndingPlaceName))
                .ToList();
        }
    }
}
