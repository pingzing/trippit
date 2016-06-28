using System.Collections.Generic;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiPlan
    {
        public long? Date { get; set; }
        public ApiPlace From { get; set; }
        public ApiPlace To { get; set; }
        public List<ApiItinerary> Itineraries { get; set; }
        public List<string> MessageEnums { get; set; }
        public List<string> MessageStrings { get; set; }
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public ApiDebugOutput? DebugOutput { get; set; }
    }
}
