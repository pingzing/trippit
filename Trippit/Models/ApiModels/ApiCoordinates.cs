using Newtonsoft.Json;

namespace Trippit.Models.ApiModels
{
    public struct ApiCoordinates
    {
        [JsonProperty("lat")]
        public float Lat { get; set; }
        [JsonProperty("lon")]
        public float Lon { get; set; }
    }
}