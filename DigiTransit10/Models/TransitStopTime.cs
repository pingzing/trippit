using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models
{
    public class TransitStopTime
    {
        public bool IsRealtime { get; set; }
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint ScheduledArrival { get; set; }
        public DateTime ScheduledArrivalDateTime => DateTime.Today + TimeSpan.FromSeconds(ScheduledArrival);
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint ScheduledDeparture { get; set; }
        public DateTime ScheduledDepartureDateTime => DateTime.Today + TimeSpan.FromSeconds(ScheduledDeparture);
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint RealtimeArrival { get; set; }
        public DateTime RealtimeArrivalDateTime => DateTime.Today + TimeSpan.FromSeconds(RealtimeArrival);
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint RealtimeDeparture { get; set; }
        public DateTime RealtimeDepartureDateTime => DateTime.Today + TimeSpan.FromSeconds(RealtimeDeparture);
        /// <summary>
        /// The human-friendly string that typically shows up in a tram or bus's sign for a given stop.
        /// </summary>
        public string StopHeadsign { get; set; }
        public ApiMode ViaMode { get; set; }
        public string ViaLineShortName { get; set; }
        public string ViaLineLongName { get; set; }
    }
}
