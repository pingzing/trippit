using System.Collections.Generic;
using Newtonsoft.Json;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiStop
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("gtfsId")]
        public string GtfsId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("lat")]
        public float Lat { get; set; }
        [JsonProperty("lon")]
        public float Lon { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("desc")]
        public string Desc { get; set; }
        [JsonProperty("zoneId")]
        public string ZoneId { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("locationType")]
        public ApiLocationType? LocationType { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("parentStation")]
        public ApiStop ParentStation { get; set; }
        [JsonProperty("wheelchairBoarding")]
        public ApiWheelchairBoarding? WheelchairBoarding { get; set; }
        [JsonProperty("direction")]
        public string Direction { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("vehicleType")]
        public int? VehicleType { get; set; }
        [JsonProperty("platformCode")]
        public string PlatformCode { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("cluster")]
        public ApiCluster Cluster { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("stops")]
        public List<ApiStop> Stops { get; set; }
        [JsonProperty("routes")]
        public List<ApiRoute> Routes { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("patterns")]
        public List<ApiPattern> Patterns { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("transfers")]
        public List<ApiStopAtDistance> Transfers{ get; set; }
        /// <summary>
        /// Just a container for a <see cref="DigiTransit10.GraphQL.GqlInlineMethodReturnValue"/>. Shouldn't be set directly.
        /// </summary>
        [JsonProperty("stoptimesWithoutPatterns")]        
        public List<ApiStoptime> StoptimesWithoutPatterns { get; set; }
    }
}