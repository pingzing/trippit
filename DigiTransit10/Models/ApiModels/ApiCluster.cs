using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiCluster
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
        /// Non-nullable.
        /// </summary>
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public List<ApiStop> Stops { get; set; }
    }
}