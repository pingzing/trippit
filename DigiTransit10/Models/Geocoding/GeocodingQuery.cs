using Newtonsoft.Json;

namespace DigiTransit10.Models.Geocoding
{
    public class GeocodingQuery
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("focus.point.lat")]
        public double FocusPointLat { get; set; }
        [JsonProperty("focus.point.lon")]
        public double FocusPointLon { get; set; }
        [JsonProperty("boundary.rect.min_lat")]
        public float BoundaryRectMinLat { get; set; }
        [JsonProperty("boundary.rect.max_lat")]
        public float BoundaryRectMaxLat { get; set; }
        [JsonProperty("boundary.rect.min_lon")]
        public float BoundaryRectMinLon { get; set; }
        [JsonProperty("boundary.rect.max_lon")]
        public float BoundaryRectMaxLon { get; set; }
        [JsonProperty("querySize")]
        public int QuerySize { get; set; }
    }
}