using Newtonsoft.Json;
using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiCluster
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
        [JsonProperty("lat")]
        public float Lat { get; set; }
        [JsonProperty("lon")]
        public float Lon { get; set; }
        [JsonProperty("stops")]
        public List<ApiStop> Stops { get; set; }
    }
}