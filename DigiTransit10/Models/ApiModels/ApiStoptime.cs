namespace DigiTransit10.Models.ApiModels
{
    public class ApiStoptime
    {
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiStop Stop { get; set; }

        public int? ScheduledArrival { get; set; }
        public int? RealtimeArrival { get; set; }
        public int? ArrivalDelay { get; set; }
        public int? ScheduledDeparture { get; set; }
        public int? RealtimeDeparture { get; set; }
        public int? DepartureDelay { get; set; }
        public bool? Timepoint { get; set; }
        public bool? Realtime { get; set; }
        public ApiEnums.ApiRealtimeState? RealtimeState { get; set; }
        public ApiEnums.ApiPickupDropoffType? PickupType { get; set; }
        public ApiEnums.ApiPickupDropoffType? DropoffType { get; set; }
        public long? ServiceDay { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiTrip Trip { get; set; }
    }
}