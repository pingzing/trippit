using Newtonsoft.Json;
using System.Collections.Generic;

namespace Trippit.Models.ApiModels
{
    public class ApiAlert
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("agency")]
        public ApiAgency Agency { get; set; }
        [JsonProperty("route")]
        public ApiRoute Route { get; set; }
        [JsonProperty("trip")]
        public ApiTrip Trip { get; set; }
        [JsonProperty("stop")]
        public ApiStop Stop { get; set; }
        [JsonProperty("patterns")]
        public List<ApiPattern> Patterns { get; set; }
        [JsonProperty("alertHeaderText")]
        public string AlertHeaderText { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("alertHeaderTextTranslations")]
        public List<ApiTranslatedString> AlertHeaderTextTranslations { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("alertDescriptionText")]
        public string AlertDescriptionText { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("alertDescriptionTextTranslations")]
        public List<ApiTranslatedString> AlertDescriptionTextTranslations { get; set; }
        [JsonProperty("alertUrl")]
        public string AlertUrl { get; set; }        
        [JsonProperty("effectiveStartDate")]
        public long? EffectiveStartDate { get; set; }        
        [JsonProperty("effectiveEndDate")]
        public long? EffectiveEndDate { get; set; }
    }
}