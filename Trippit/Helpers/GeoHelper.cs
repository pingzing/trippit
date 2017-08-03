using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace Trippit.Helpers
{
    public static class GeoHelper
    {
        private const double Circle = 2 * Math.PI;
        private const double DegreesToRadians = Math.PI / 180.0;
        private const double RadiansToDegrees = 180.0 / Math.PI;
        private const double EarthRadiusMeters = 6378137.0;
        public static IList<Geopoint> GetGeocirclePoints(Geopoint center, double radiusInMeters, int numberOfPoints = 50)
        {
            var locations = new List<Geopoint>();
            double latA = center.Position.Latitude * DegreesToRadians;
            double lonA = center.Position.Longitude * DegreesToRadians;
            double angularDistance = radiusInMeters / EarthRadiusMeters;

            double sinLatA = Math.Sin(latA);
            double cosLatA = Math.Cos(latA);
            double sinDistance = Math.Sin(angularDistance);
            double cosDistance = Math.Cos(angularDistance);
            double sinLatAMultCosDistance = sinLatA * cosDistance;
            double cosLatAMultSinDistance = cosLatA * sinDistance;

            double step = Circle / numberOfPoints;
            for (double angle = 0; angle < Circle; angle += step)
            {
                var lat = Math.Asin(sinLatAMultCosDistance + cosLatAMultSinDistance * Math.Cos(angle));
                var dlon = Math.Atan2(Math.Sin(angle) * cosLatAMultSinDistance, cosDistance - sinLatA * Math.Sin(lat));
                var lon = ((lonA + dlon + Math.PI) % Circle) - Math.PI;

                locations.Add(new Geopoint(new BasicGeoposition
                {
                    Latitude = lat * RadiansToDegrees,
                    Longitude = lon * RadiansToDegrees
                }));
            }
            return locations;
        }
    }
}
