using System;
using System.Collections.Generic;
using Trippit.Models.ApiModels;

namespace Trippit.Models
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
        public WalkingAmount WalkAmount { get; set; }
        public WalkingSpeed WalkSpeed { get; set; }

        public TripQueryDetails(ApiCoordinates fromCoords, string fromString, 
            List<ApiCoordinates> intermediateCoords, ApiCoordinates toCoords, string toString, 
            TimeSpan time, DateTime date, bool isTimeTypeArrival, string transit,
            WalkingAmount walkAmount, WalkingSpeed walkSpeed)
        {
            FromPlaceCoords = fromCoords;
            FromPlaceString = fromString;
            IntermediateCoords = intermediateCoords;
            ToPlaceCoordinates = toCoords;
            ToPlaceString = toString;
            TransitModes = transit;
            Time = time;
            Date = date;
            IsTimeTypeArrival = isTimeTypeArrival;
            WalkAmount = walkAmount;
            WalkSpeed = walkSpeed;
        }
    }
}
