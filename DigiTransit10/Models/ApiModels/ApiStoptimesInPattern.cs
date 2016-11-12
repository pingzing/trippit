using Newtonsoft.Json;
using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiStoptimesInPattern
    {
        [JsonProperty("pattern")]
        public ApiPattern Pattern { get; set; }

        [JsonProperty("stoptimes")]
        public List<ApiStoptime> Stoptimes { get; set; }
    }
}