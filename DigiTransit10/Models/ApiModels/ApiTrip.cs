using System.Collections.Generic;
using Newtonsoft.Json;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiTrip
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("gtfsId")]
        public string GtfsId { get; set; }
        [JsonProperty("route")]
        public ApiRoute Route { get; set; }
        [JsonProperty("serviceId")]
        public string ServiceId { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("activeDates")]
        public List<string> ActiveDates { get; set; }
        [JsonProperty("tripShortName")]
        public string TripShortName { get; set; }
        [JsonProperty("tripHeadsign")]
        public string TripHeadsign { get; set; }
        [JsonProperty("routeShortName")]
        public string RouteShortName { get; set; }
        [JsonProperty("directionId")]
        public string DirectionId { get; set; }
        [JsonProperty("blockId")]
        public string BlockId { get; set; }
        [JsonProperty("shapeId")]
        public string ShapeId { get; set; }
        [JsonProperty("wheelchairAccessible")]
        public ApiWheelchairBoarding? WheelchairAccessible { get; set; }
        [JsonProperty("bikesAllowed")]
        public ApiBikesAllowed? BikesAllowed { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("pattern")]
        public ApiPattern Pattern { get; set; }
        [JsonProperty("stops")]
        public List<ApiStop> Stops { get; set; }
        [JsonProperty("semanticHash")]
        public List<ApiStop> SemanticHash { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("stoptimes")]
        public List<ApiStoptime> Stoptimes { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("geometry")]
        public List<float[]> Geometry { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("alerts")]
        public List<ApiAlert> Alerts { get; set; }
    }
}