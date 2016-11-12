using Newtonsoft.Json;
using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiPattern
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("route")]
        public ApiRoute Route { get; set; }
        [JsonProperty("directionId")]
        public int? DirectionId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("headsign")]
        public string HeadSign { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("trips")]
        public List<ApiTrip> Trips{ get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("stops")]
        public List<ApiStop> Stops { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("geometry")]
        public List<ApiCoordinates> Geometry { get; set; }
        [JsonProperty("semanticHash")]
        public string SemanticHash { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("alerts")]
        public List<ApiAlert> Alerts { get; set; }
    }
}