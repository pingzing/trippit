using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiPattern
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Id { get; set; }
        public ApiRoute Route { get; set; }       
        public int? DirectionId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Code { get; set; }
        public string HeadSign { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public List<ApiTrip> Trips{ get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public List<ApiStop> Stops { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiCoordinates> Geometry { get; set; }
        public string SemanticHash { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiAlert> Alerts { get; set; }
    }
}