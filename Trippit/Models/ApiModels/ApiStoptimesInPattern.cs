using Newtonsoft.Json;
using System.Collections.Generic;

namespace Trippit.Models.ApiModels
{
    public class ApiStoptimesInPattern
    {
        [JsonProperty("pattern")]
        public ApiPattern Pattern { get; set; }

        [JsonProperty("stoptimes")]
        public List<ApiStoptime> Stoptimes { get; set; }
    }
}