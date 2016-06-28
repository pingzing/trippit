﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiPlace
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
    }
}