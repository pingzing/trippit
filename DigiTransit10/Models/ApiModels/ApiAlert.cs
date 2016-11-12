using Newtonsoft.Json;
using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiAlert
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("agency")]
        public ApiAgency Agency { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("route")]
        public ApiRoute Route { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("trip")]
        public ApiTrip Trip { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("stop")]
        public ApiStop Stop { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("patterns")]
        public List<ApiPattern> Patterns { get; set; }
        [JsonProperty("alertHeaderText")]
        public string AlertHeaderText { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("alertHeaderTextTranslations")]
        public List<string> AlertHeaderTextTranslations { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("alertDescriptionText")]
        public string AlertDescriptionText { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("alertDescriptionTextTranslations")]
        public string AlertDescriptionTextTranslations { get; set; }
        [JsonProperty("alertUrl")]
        public string AlertUrl { get; set; }
        /// <summary>
        /// UNIX-time? todo:check
        /// </summary>
        [JsonProperty("effectiveStartDate")]
        public long? EffectiveStartDate { get; set; }
        /// <summary>
        /// UNIX-time? todo:check
        /// </summary>
        [JsonProperty("effectiveEndDate")]
        public long? EffectiveEndDate { get; set; }
    }
}