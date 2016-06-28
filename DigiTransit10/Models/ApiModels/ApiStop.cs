using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiStop
    {
        public string Id { get; set; }
        public string GtfsId { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
        public string ZoneId { get; set; }
        public string Url { get; set; }
        public ApiEnums.ApiLocationType? LocationType { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiStop ParentStation { get; set; }
        public ApiEnums.ApiWheelchairBoarding? WheelchairBoarding { get; set; }
        public string Direction { get; set; }
        public string Timezone { get; set; }
        public int? VehicleType { get; set; }
        public string PlatformCode { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiCluster Cluster { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiStop> Stops { get; set; }
        public List<ApiRoute> Routes { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiPattern> Patterns { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiStopAtDistance> Transfers{ get; set; }
    }
}