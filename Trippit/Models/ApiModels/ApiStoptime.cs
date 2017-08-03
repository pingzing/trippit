using Newtonsoft.Json;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Models.ApiModels
{
    public class ApiStoptime
    {
        [JsonProperty("stop")]
        public ApiStop Stop { get; set; }
        /// <summary>
        /// Seconds since 00:00 of this day.
        /// </summary>
        [JsonProperty("scheduledArrival")]
        public int? ScheduledArrival { get; set; }
        /// <summary>
        /// Seconds since 00:00 of this day.
        /// </summary>
        [JsonProperty("realtimeArrival")]
        public int? RealtimeArrival { get; set; }
        [JsonProperty("arrivalDelay")]
        public int? ArrivalDelay { get; set; }
        /// <summary>
        /// Seconds since 00:00 of this day.
        /// </summary>
        [JsonProperty("scheduledDeparture")]
        public int? ScheduledDeparture { get; set; }
        /// <summary>
        /// Seconds since 00:00 of this day.
        /// </summary>
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
        [JsonProperty("trip")]
        public ApiTrip Trip { get; set; }
        [JsonProperty("stopHeadsign")]
        public string StopHeadsign { get; set; }
    }
}