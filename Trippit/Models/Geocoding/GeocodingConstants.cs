namespace Trippit.Models.Geocoding
{
    public static class GeocodingConstants
    {
        // These values all acquired by snooping on a request made on the DigiTransit website. They determine the bounding box that get used for 
        // the Geocoding address search.

        public const double BoundaryRectMinLat = 59.9;
        public const double BoundaryRectMaxLat = 60.45;
        public const double BoundaryRectMinLon = 24.3;
        public const double BoundaryRectMaxLon = 25.5;
        public const double FocusPointLat = 60.229070899999996;
        public const double FocusPointLon = 25.1289026;
    }
}
