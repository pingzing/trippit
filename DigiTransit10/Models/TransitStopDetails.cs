using DigiTransit10.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            // Consolidate all the duplicates we get from the network call.
            // TODO: Investigate why we get dupes at all. Is it our fault, or the server's fault?
            LinesThroughStop = stop.StoptimesForServiceDate
                .Where(x => x.Stoptimes.Any())
                .GroupBy(x => x.Pattern.Route.GtfsId)
                .Select(x => {
                    ApiStoptimesInPattern stoptimes = x.First();
                    return new TransitLineWithoutStops
                    {
                        GtfsId = stoptimes.Pattern.Route.GtfsId,
                        LongName = stoptimes.Pattern.Route.LongName,
                        ShortName = stoptimes.Pattern.Route.ShortName,
                        TransitMode = stoptimes.Pattern.Route.Mode
                    };
                })               
                .ToList();

            Stoptimes = stop.StoptimesForServiceDate.SelectMany(
                x => x.Stoptimes                   
                .Where(z => !String.IsNullOrWhiteSpace(z.StopHeadsign))
                .Select(y => new TransitStopTime
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
