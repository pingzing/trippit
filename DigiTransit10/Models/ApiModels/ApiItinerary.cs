using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiItinerary
    {
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
        public long? Duration { get; set; }
        public long WaitingTime { get; set; }
        public long? WalkTime { get; set; }
        public long? WalkDistance { get; set; }        
        public List<ApiLeg> Legs { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiFare> Fares { get; set; }
    }
}