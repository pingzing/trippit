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
        public Dictionary<TransitLineWithoutStops, List<TransitStopTime>> LinesThroughStop { get; set; }

        public TransitStopDetails(ApiStop stop, DateTime forDate)
        {
            GtfsId = stop.GtfsId;
            Name = stop.Name;
            ForDate = forDate;
            LinesThroughStop = stop.StoptimesForServiceDate.ToDictionary(
                keySelector: x =>                                                    
                    new TransitLineWithoutStops
                    {
                        LongName = x.Pattern.Route.LongName,
                        ShortName = x.Pattern.Route.ShortName,
                        TransitMode = x.Pattern.Route.Mode
                    },
                elementSelector: x =>
                    x.Stoptimes.Select(y => new TransitStopTime
                    {
                        IsRealtime = y.Realtime.Value,
                        RealtimeArrival = (uint)y.RealtimeArrival.Value,
                        RealtimeDeparture = (uint)y.RealtimeDeparture.Value,
                        ScheduledArrival = (uint)y.ScheduledArrival.Value,
                        ScheduledDeparture = (uint)y.ScheduledDeparture.Value,
                        StopHeadsign = y.StopHeadsign
                    }).ToList()
                );
        }
    }
}
