using System.Collections.Generic;
using Newtonsoft.Json;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiLeg
    {
        [JsonProperty("startTime")]
        public long? StartTime { get; set; }
        [JsonProperty("endTime")]
        public long? EndTime { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("mode")]
        public ApiMode? Mode { get; set; }
        [JsonProperty("duration")]
        public long? Duration { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("legGeometry")]
        public ApiLegGeometry LegGeometry { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("agency")]
        public ApiAgency Agency { get; set; }
        [JsonProperty("realtime")]
        public bool? Realtime { get; set; }
        [JsonProperty("distance")]
        public float? Distance { get; set; }
        [JsonProperty("transitLeg")]
        public bool? TransitLeg { get; set; }
        [JsonProperty("rentedBike")]
        public bool? RentedBike { get; set; }
        [JsonProperty("from")]
        public ApiPlace From { get; set; }
        [JsonProperty("to")]
        public ApiPlace To { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("route")]
        public ApiRoute Route { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("trip")]
        public ApiTrip Trip { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("intermediateStops")]
        public List<ApiStop> IntermediateStops { get; set; }
    }
}