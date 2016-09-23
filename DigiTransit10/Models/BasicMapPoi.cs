using System;
using Windows.Devices.Geolocation;

namespace DigiTransit10.Models
{
    public class BasicMapPoi : IMapPoi
    {
        public BasicGeoposition Coords { get; set; }
        public string Name { get; set; }

        public Guid OptionalId{ get; set; }
    }
}
