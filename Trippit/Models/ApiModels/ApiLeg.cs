using Newtonsoft.Json;
using System.Collections.Generic;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Models.ApiModels
{
    public class ApiLeg
    {
        [JsonProperty("startTime")]
        public long? StartTime { get; set; }
        [JsonProperty("endTime")]
        public long? EndTime { get; set; }
        [JsonProperty("mode")]
        public ApiMode? Mode { get; set; }
        [JsonProperty("duration")]
        public long? Duration { get; set; }
        [JsonProperty("legGeometry")]
        public ApiLegGeometry LegGeometry { get; set; }
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
        [JsonProperty("route")]
        public ApiRoute Route { get; set; }
        [JsonProperty("trip")]
        public ApiTrip Trip { get; set; }
        [JsonProperty("intermediateStops")]
        public List<ApiStop> IntermediateStops { get; set; }
    }
}