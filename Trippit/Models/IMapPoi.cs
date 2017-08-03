using System;
using Windows.Devices.Geolocation;

namespace Trippit.Models
{
    public interface IMapPoi
    {
        string Name { get; set; }
        BasicGeoposition Coords { get; }
        Guid Id { get; set; }
    }
}
