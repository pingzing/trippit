using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiDataContainer
    {
        [JsonProperty("data")]
        public JObject Data { get; set; }
    }
}
