using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiRoute
    {        
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string GtfsId { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiAgency Agency { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Type { get; set; }
        public string Desc { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public ApiEnums.ApiBikesAllowed? BikesAllowed { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiPattern> Patterns { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiStop> Stops { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiTrip> Trips { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiAlert> Alerts { get; set; }
    }
}