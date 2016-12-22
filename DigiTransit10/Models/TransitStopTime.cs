using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Models
{
    public class TransitStopTime
    {
        public bool IsRealtime { get; set; }
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint ScheduledArrival { get; set; }
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint ScheduledDeparture { get; set; }
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint RealtimeArrival { get; set; }
        /// <summary>
        /// In seconds since 00:00 that morning.
        /// </summary>
        public uint RealtimeDeparture { get; set; }
        /// <summary>
        /// The human-friendly string that typically shows up in a tram or bus's sign for a given stop.
        /// </summary>
        public string StopHeadsign { get; set; }
    }
}
