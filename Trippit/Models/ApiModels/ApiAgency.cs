using Newtonsoft.Json;
using System.Collections.Generic;

namespace Trippit.Models.ApiModels
{
    public class ApiAgency
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
        /// Non-nullable.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("fareUrl")]
        public string FareUrl { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("routes")]
        public List<ApiRoute> Routes { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("alerts")]
        public List<ApiAlert> Alerts { get; set; }
    }
}