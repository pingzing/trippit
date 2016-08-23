using System.Collections.Generic;
using DigiTransit10.Models.ApiModels;

namespace DigiTransit10.Models
{
    public class DetailedTripListLeg
    {
        public static DetailedTripListLeg FromApiLeg(ApiLeg leg)
        {
            return new DetailedTripListLeg
            {
                StartTime = leg.StartTime.Value,
                EndTime = leg.EndTime.Value,
                FromName = leg.From.Name,
                ToName = leg.To.Name,
                ShortName = leg.Route?.ShortName,
                Distance = leg.Distance.Value,
                Mode = leg.Mode.Value,
                IntermediateStops = leg.IntermediateStops,
                IsEnd = false
            };
        }

        public static DetailedTripListLeg ApiLegToEndLeg(ApiLeg leg)
        {
            return new DetailedTripListLeg
            {
                EndTime = leg.EndTime.Value,
                ToName = leg.To.Name,
                IsEnd = true
            };
        }

        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string ShortName { get; set; }
        public float Distance { get; set; }
        public ApiEnums.ApiMode Mode { get; set; }
        public List<ApiStop> IntermediateStops { get; set; }

        public bool IsEnd { get; set; }
    }
}