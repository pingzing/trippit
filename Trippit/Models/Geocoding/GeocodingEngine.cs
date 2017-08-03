using Newtonsoft.Json;

namespace Trippit.Models.Geocoding
{
    public class GeocodingEngine
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}