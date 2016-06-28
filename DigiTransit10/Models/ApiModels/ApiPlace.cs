namespace DigiTransit10.Models.ApiModels
{
    public class ApiPlace
    {
        public string Name { get; set; }
        public ApiEnums.ApiVertexType? VertexType { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiStop Stop { get; set; }
        /// <summary>
        /// Nullable.
        /// </summary>
        public ApiBikeRentalStation BikeRentalStation { get; set; }
    }
}