using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Models.Geocoding
{
    public class Geocoding
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("attribution")]
        public string Attribution { get; set; }        
        [JsonProperty("query")]
        public GeocodingQuery Query { get; set; }
        [JsonProperty("engine")]
        public GeocodingEngine Engine { get; set; }
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
}
