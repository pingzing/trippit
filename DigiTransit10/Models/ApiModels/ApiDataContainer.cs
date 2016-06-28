using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
