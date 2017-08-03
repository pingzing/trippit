using Newtonsoft.Json;
using System.Collections.Generic;

namespace Trippit.Models.ApiModels
{
    public class ApiStopAtDistanceConnection
    {
        [JsonProperty("edges")]
        public List<ApiStopAtDistanceEdge> Edges { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("pageInfo")]
        public ApiPageInfo PageInfo { get; set; }
    }
}
