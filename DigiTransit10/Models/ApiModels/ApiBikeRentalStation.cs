using System.Collections.Generic;
using Newtonsoft.Json;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiBikeRentalStation
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("stationId")]
        public string StationId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("bikesAvailable")]
        public int? BikesAvailable { get; set; }
        [JsonProperty("spacesAvailable")]
        public int? SpacesAvailable { get; set; }
        [JsonProperty("realtime")]
        public bool? Realtime { get; set; }
        [JsonProperty("allowDropoff")]
        public bool? AllowDropoff { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("networks")]
        public List<string> Networks { get; set; }
        [JsonProperty("long")]
        public float? Lon { get; set; }
        [JsonProperty("lat")]
        public float? Lat { get; set; }
    }
}