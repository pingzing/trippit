using Newtonsoft.Json;

namespace DigiTransit10.Models.Geocoding
{
    public class GeocodingGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }
}