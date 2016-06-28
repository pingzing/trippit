using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiTrip
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string GtfsId { get; set; }        
        public ApiRoute Route { get; set; }
        public string ServiceId { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<string> ActiveDates { get; set; }

        public string TripShortName { get; set; }
        public string TripHeadsign { get; set; }
        public string RouteShortName { get; set; }
        public string DirectionId { get; set; }
        public string BlockId { get; set; }
        public string ShapeId { get; set; }
        public ApiEnums.ApiWheelchairBoarding? WheelchairAccessible { get; set; }
        public ApiEnums.ApiBikesAllowed? BikesAllowed { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiPattern Pattern { get; set; }

        public List<ApiStop> Stops { get; set; }
        public List<ApiStop> SemanticHash { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiStoptime> Stoptimes { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<float[]> Geometry { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiAlert> Alerts { get; set; }
    }
}