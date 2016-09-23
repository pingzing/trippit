using System;
using Windows.Devices.Geolocation;

namespace DigiTransit10.Models
{
    public interface IMapPoi
    {
        string Name { get; set; }
        BasicGeoposition Coords { get; }
        Guid OptionalId { get; set; }
    }
}
