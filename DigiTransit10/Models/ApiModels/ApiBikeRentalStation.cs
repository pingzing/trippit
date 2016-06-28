using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiBikeRentalStation
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Id { get; set; }
        public string StationId { get; set; }
        public string Name { get; set; }
        public int? BikesAvailable { get; set; }
        public int? SpacesAvailable { get; set; }
        public bool? Realtime { get; set; }
        public bool? AllowDropoff { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<string> Networks { get; set; }
        public float? Lon { get; set; }
        public float? Lat { get; set; }
    }
}