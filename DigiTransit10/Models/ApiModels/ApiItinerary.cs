﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace DigiTransit10.Models.ApiModels
{
    public class ApiItinerary
    {
        [JsonProperty("startTime")]
        public long? StartTime { get; set; }
        [JsonProperty("endTime")]
        public long? EndTime { get; set; }
        [JsonProperty("duration")]
        public long? Duration { get; set; }
        [JsonProperty("waitingTime")]
        public long WaitingTime { get; set; }
        [JsonProperty("walkTime")]
        public long? WalkTime { get; set; }
        [JsonProperty("walkDistance")]
        public long? WalkDistance { get; set; }
        [JsonProperty("legs")]
        public List<ApiLeg> Legs { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        [JsonProperty("fares")]
        public List<ApiFare> Fares { get; set; }
    }
}