using Newtonsoft.Json;

namespace DigiTransit10.Models.ApiModels
{
    public struct ApiFare
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("cents")]
        public int Cents { get; set; }
    }
}