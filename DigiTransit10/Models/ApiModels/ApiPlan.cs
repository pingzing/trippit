using Newtonsoft.Json;
using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    [JsonObject(Title = "plan")]
    public class ApiPlan
    {
        [JsonProperty("date")]
        public long? Date { get; set; }
        [JsonProperty("from")]
        public ApiPlace From { get; set; }
        [JsonProperty("to")]
        public ApiPlace To { get; set; }
        [JsonProperty("itineraries")]
        public List<ApiItinerary> Itineraries { get; set; }
        [JsonProperty("messageEnums")]
        public List<string> MessageEnums { get; set; }
        [JsonProperty("messageStrings")]
        public List<string> MessageStrings { get; set; }
        [JsonProperty("debugOutput")]
        public ApiDebugOutput? DebugOutput { get; set; }
    }
}
