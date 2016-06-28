namespace DigiTransit10.Models.ApiModels
{
    public struct ApiFare
    {
        public string Type { get; set; }
        public string Currency { get; set; }
        public int Cents { get; set; }
    }
}