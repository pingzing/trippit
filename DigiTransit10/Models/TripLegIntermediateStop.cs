using Windows.Devices.Geolocation;

namespace DigiTransit10.Models
{
    public class TripLegIntermediateStop
    {
        public string Name { get; set; }
        public BasicGeoposition Coords { get; set; }
    }
}