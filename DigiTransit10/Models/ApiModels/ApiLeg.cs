using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiLeg
    {
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiEnums.ApiMode? Mode { get; set; }
        public long? Duration { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiLegGeometry LegGeometry { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiAgency Agency { get; set; }
        public bool? Realtime { get; set; }
        public float? Distance { get; set; }
        public bool? TransitLeg { get; set; }
        public bool? RentedBike { get; set; }
        public ApiPlace From { get; set; }
        public ApiPlace To { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiRoute Route { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiTrip Trip { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiStop> IntermediateStops { get; set; }
    }
}