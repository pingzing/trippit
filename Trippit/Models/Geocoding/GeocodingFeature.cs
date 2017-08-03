using Newtonsoft.Json;

namespace Trippit.Models.Geocoding
{
    public class GeocodingFeature
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("geometry")]
        public GeocodingGeometry Geometry { get; set; }
        [JsonProperty("properties")]
        public GeocodingProperties Properties { get; set; }
    }
}