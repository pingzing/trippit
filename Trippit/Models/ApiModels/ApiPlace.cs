using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using Windows.Devices.Geolocation;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Models.ApiModels
{
    public class ApiPlace : IMapPoi
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("vertexType")]
        public ApiVertexType? VertexType { get; set; }
        [JsonProperty("lat")]
        public float Lat { get; set; }
        [JsonProperty("lon")]
        public float Lon { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("stop")]
        public ApiStop Stop { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("bikeRentalStation")]
        public ApiBikeRentalStation BikeRentalStation { get; set; }

        [JsonIgnore]
        public BasicGeoposition Coords => new BasicGeoposition { Altitude = 0.0, Latitude = Lat, Longitude = Lon };

        [JsonIgnore]
        public Guid Id { get; set; }
    }
}