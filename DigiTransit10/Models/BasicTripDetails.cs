using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using DigiTransit10.Models.ApiModels;

namespace DigiTransit10.Models
{
    public class BasicTripDetails
    {
        public TimeSpan Time { get; set; }
        public DateTime Date { get; set; }
        public string FromPlaceString { get; set; }
        public string ToPlaceString { get; set; }
        public bool IsTimeTypeArrival { get; set; }
        public ApiCoordinates FromPlaceCoords { get; set; }
        public ApiCoordinates ToPlaceCoordinates { get; set; }

        public BasicTripDetails(string fromString, string toString, TimeSpan time, DateTime date, bool isTimeTypeArrival)
        {
            FromPlaceString = fromString;
            ToPlaceString = toString;
            FillRemainingDetails(time, date, isTimeTypeArrival);
        }

        public BasicTripDetails(ApiCoordinates fromCoords, ApiCoordinates toCoords, TimeSpan time, DateTime date, bool isTimeTypeArrival)
        {
            FromPlaceCoords = fromCoords;
            ToPlaceCoordinates = toCoords;
            FillRemainingDetails(time, date, isTimeTypeArrival);
        }

        private void FillRemainingDetails(TimeSpan time, DateTime date, bool isTimeTypeArrival)
        {
            Time = time;
            Date = date;
            IsTimeTypeArrival = isTimeTypeArrival;
        }
    }
    
}
