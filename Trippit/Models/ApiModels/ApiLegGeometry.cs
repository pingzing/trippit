using Newtonsoft.Json;

namespace Trippit.Models.ApiModels
{
    public struct ApiLegGeometry
    {
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("points")]
        public string Points { get; set; }
    }
}