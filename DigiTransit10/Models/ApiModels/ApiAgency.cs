using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiAgency
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
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Timezone { get; set; }

        public string Lang { get; set; }
        public string Phone { get; set; }
        public string FareUrl { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiRoute> Routes { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiAlert> Alerts { get; set; }
    }
}