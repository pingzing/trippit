namespace DigiTransit10.Models.ApiModels
{
    public class ApiStopAtDistance
    {
        /// <summary>
        /// Non-nullable.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiStop Stop { get; set; }
        public int Distance { get; set; }
    }
}