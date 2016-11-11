using Windows.Devices.Geolocation;

namespace DigiTransit10.Models
{
    public class TransitStop
    {
        public string Name { get; set; }
        public BasicGeoposition Coords { get; set; }
    }
}