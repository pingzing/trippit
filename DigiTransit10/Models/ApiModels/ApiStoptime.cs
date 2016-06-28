﻿using Newtonsoft.Json;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiStoptime
    {
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("stop")]
        public ApiStop Stop { get; set; }
        [JsonProperty("scheduledArrival")]
        public int? ScheduledArrival { get; set; }
        [JsonProperty("realtimeArrival")]
        public int? RealtimeArrival { get; set; }
        [JsonProperty("arrivalDelay")]
        public int? ArrivalDelay { get; set; }
        [JsonProperty("scheduledDeparture")]
        public int? ScheduledDeparture { get; set; }
        [JsonProperty("realtimeDeparture")]
        public int? RealtimeDeparture { get; set; }
        [JsonProperty("departureDelay")]
        public int? DepartureDelay { get; set; }
        [JsonProperty("timepoint")]
        public bool? Timepoint { get; set; }
        [JsonProperty("realtime")]
        public bool? Realtime { get; set; }
        [JsonProperty("realtimeState")]
        public ApiRealtimeState? RealtimeState { get; set; }
        [JsonProperty("pickupType")]
        public ApiPickupDropoffType? PickupType { get; set; }
        [JsonProperty("dropoffType")]
        public ApiPickupDropoffType? DropoffType { get; set; }
        [JsonProperty("serviceDay")]
        public long? ServiceDay { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("trip")]
        public ApiTrip Trip { get; set; }
    }
}