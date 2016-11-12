using Newtonsoft.Json;

namespace DigiTransit10.Models.Geocoding
{
    public class GeocodingResponse
    {
        [JsonProperty("geocoding")]
        public Geocoding Geocoding { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("features")]
        public GeocodingFeature[] Features { get; set; }
        [JsonProperty("bbox")]
        public double[] BoundingBox { get; set; }
    }
}
