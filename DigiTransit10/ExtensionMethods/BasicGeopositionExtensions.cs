using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace DigiTransit10.ExtensionMethods
{
    public static class BasicGeopositionExtensions
    {
        public static bool Equals(this BasicGeoposition geo, BasicGeoposition other)
        {
            return geo.Altitude == other.Altitude
                && geo.Latitude == other.Latitude
                && geo.Longitude == other.Longitude;
        }        

        public static BasicGeoposition Create(double altitude, double longitutde, double latitude)
        {
            return new BasicGeoposition
            {
                 Altitude = altitude,
                 Latitude = latitude,
                 Longitude = longitutde
            };
        }
    }
}
