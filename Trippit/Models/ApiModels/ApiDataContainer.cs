using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Trippit.Models.ApiModels
{
    public class ApiDataContainer
    {
        [JsonProperty("data")]
        public JObject Data { get; set; }
    }
}
