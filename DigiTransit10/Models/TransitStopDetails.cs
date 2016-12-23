using DigiTransit10.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigiTransit10.Models
{
    public class TransitStopDetails
    {
        public string GtfsId { get; set; }
        public string Name { get; set; }
        public DateTime ForDate { get; set; }
        public List<TransitLineWithoutStops> LinesThroughStop { get; set; }
        public List<TransitStopTime> Stoptimes { get; set; }

        public TransitStopDetails(ApiStop stop, DateTime forDate)
        {
            GtfsId = stop.GtfsId;
            Name = stop.Name;
            ForDate = forDate;
            LinesThroughStop = stop.StoptimesForServiceDate.Select(
                x => new TransitLineWithoutStops
                {
                    LongName = x.Pattern.Route.LongName,
                    ShortName = x.Pattern.Route.ShortName,
                    TransitMode = x.Pattern.Route.Mode
                })
                .ToList();
            Stoptimes = stop.StoptimesForServiceDate.SelectMany(
                x => x.Stoptimes.Select(y => new TransitStopTime
                {
                    IsRealtime = y.Realtime.Value,
                    RealtimeArrival = (uint)y.RealtimeArrival.Value,
                    RealtimeDeparture = (uint)y.RealtimeDeparture.Value,
                    ScheduledArrival = (uint)y.ScheduledArrival.Value,
                    ScheduledDeparture = (uint)y.ScheduledDeparture.Value,
                    StopHeadsign = y.StopHeadsign,
                    ViaLineShortName = x.Pattern.Route.ShortName,
                    ViaLineLongName = x.Pattern.Route.LongName,
                    ViaMode = x.Pattern.Route.Mode
                }))
                .OrderBy(x => x.ScheduledDeparture)
                .ToList();               
        }
    }
}
