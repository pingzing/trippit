using Newtonsoft.Json;

namespace Trippit.Models.ApiModels
{
    public struct ApiDebugOutput
    {
        [JsonProperty("totalTime")]
        public long? TotalTime { get; set; }
    }
}