using Newtonsoft.Json;

namespace DigiTransit10.Models.ApiModels
{
    public struct ApiDebugOutput
    {
        [JsonProperty("totalTime")]
        public long? TotalTime { get; set; }
    }
}