using Newtonsoft.Json;

namespace Trippit.Models.Geocoding
{
    public class GeocodingGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }
}