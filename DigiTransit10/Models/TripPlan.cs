using DigiTransit10.Models.ApiModels;

namespace DigiTransit10.Models
{
    public class TripPlan
    {
        public ApiPlan ApiPlan { get; set; }
        public string StartingPlaceName { get; set; }
        public string EndingPlaceName { get; set; }
    }
}
