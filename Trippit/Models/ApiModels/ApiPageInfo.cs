using Newtonsoft.Json;

namespace Trippit.Models.ApiModels
{
    public class ApiPageInfo
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        [JsonProperty("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }
        public string StartCursor { get; set; }
        public string EndCursor { get; set; }
    }
}