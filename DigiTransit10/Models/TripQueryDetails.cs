using DigiTransit10.Models.ApiModels;
using System;
using System.Collections.Generic;

namespace DigiTransit10.Models
{
    public class TripQueryDetails
    {
        public TimeSpan Time { get; set; }
        public DateTime Date { get; set; }
        public string FromPlaceString { get; set; }
        public string ToPlaceString { get; set; }
        public bool IsTimeTypeArrival { get; set; }
        public ApiCoordinates FromPlaceCoords { get; set; }
        public List<ApiCoordinates> IntermediateCoords { get; set; }
        public ApiCoordinates ToPlaceCoordinates { get; set; }
        public string TransitModes { get; set;}

        public TripQueryDetails(ApiCoordinates fromCoords, List<ApiCoordinates> intermediateCoords,
            ApiCoordinates toCoords, TimeSpan time, DateTime date, bool isTimeTypeArrival, string transit)
        {
            FromPlaceCoords = fromCoords;
            IntermediateCoords = intermediateCoords;
            ToPlaceCoordinates = toCoords;
            TransitModes = transit;
            Time = time;
            Date = date;
            IsTimeTypeArrival = isTimeTypeArrival;
        }
    }
}
