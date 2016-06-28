using System.Collections.Generic;
using Newtonsoft.Json;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiRoute
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
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("agency")]
        public ApiAgency Agency { get; set; }
        [JsonProperty("shortName")]
        public string ShortName { get; set; }
        [JsonProperty("longName")]
        public string LongName { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("desc")]
        public string Desc { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("textColor")]
        public string TextColor { get; set; }
        [JsonProperty("bikesAllowed")]
        public ApiBikesAllowed? BikesAllowed { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("patterns")]
        public List<ApiPattern> Patterns { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("stops")]
        public List<ApiStop> Stops { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("trips")]
        public List<ApiTrip> Trips { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("alerts")]
        public List<ApiAlert> Alerts { get; set; }
    }
}