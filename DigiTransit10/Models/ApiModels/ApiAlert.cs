using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiAlert
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiAgency Agency { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiRoute Route { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiTrip Trip { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiStop Stop { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public List<ApiPattern> Patterns { get; set; }

        public string AlertHeaderText { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public List<string> AlertHeaderTextTranslations { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string AlertDescriptionText { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string AlertDescriptionTextTranslations { get; set; }

        public string AlertUrl { get; set; }
        /// <summary>
        /// UNIX-time? todo:check
        /// </summary>
        public long? EffectiveStartDate { get; set; }
        /// <summary>
        /// UNIX-time? todo:check
        /// </summary>
        public long? EffectiveEndDate { get; set; }
    }
}