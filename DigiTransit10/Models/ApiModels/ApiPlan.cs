using System.Collections.Generic;
using Newtonsoft.Json;

namespace DigiTransit10.Models.ApiModels
{    
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
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("debugOutput")]
        public ApiDebugOutput? DebugOutput { get; set; }
    }
}
