using Newtonsoft.Json;

namespace Trippit.Models.Geocoding
{
    public class GeocodingProperties
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("gid")]
        public string Gid { get; set; }
        [JsonProperty("layer")]
        public string Layer { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("source_id")]
        public string SourceId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("housenumber")]
        public string HouseNumber { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("confidence")]
        public double Confidence { get; set; }
        [JsonProperty("distance")]
        public double Distance { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_gid")]
        public string CountryGid { get; set; }
        [JsonProperty("country_a")]
        public string CountryA { get; set; }
        [JsonProperty("macroregion")]
        public string MacroRegion { get; set; }
        [JsonProperty("macroregion_gid")]
        public string MacroRegionGid { get; set; }
        [JsonProperty("locality")]
        public string Locality { get; set; }
        [JsonProperty("locality_gid")]
        public string LocalityGid { get; set; }
        [JsonProperty("neighborhood")]
        public string Neighborhood { get; set; }
        [JsonProperty("neighborhood_gid")]
        public string NeighborhoodGid { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}