﻿using DigiTransit10.Models.ApiModels;

namespace DigiTransit10.Models.TripPlanStrip
{
    public class ItineraryModel
    {
        public ApiItinerary BackingItinerary { get; set; }
        public string StartingPlaceName { get; set; }
        public string EndingPlaceName { get; set; }
    }
}
