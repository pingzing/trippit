using Newtonsoft.Json;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiStopAtDistanceEdge
    {
        [JsonProperty("node")]
        public ApiStopAtDistance Node { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("cursor")]
        public string Cursor { get; set; }
    }
}