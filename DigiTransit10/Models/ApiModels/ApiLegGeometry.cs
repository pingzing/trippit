using Newtonsoft.Json;

namespace DigiTransit10.Models.ApiModels
{
    public struct ApiLegGeometry
    {
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("points")]
        public string Points { get; set; }
    }
}