using DigiTransit10.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;

namespace DigiTransit10.Helpers
{
    public static class GooglePolineDecoder
    {
        //Courtesy of http://www.codeproject.com/Tips/312248/Google-Maps-Direction-API-V-Polyline-Decoder, subject to the Code Project Open License: http://www.codeproject.com/info/cpol10.aspx
        /// <summary>
        /// Decode a Google Poline-Encoded list of lat-long coordinates into a <see cref="List{T}"/> of <see cref="ApiCoordinates"/>.
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static List<BasicGeoposition> Decode(string encoded)
        {
            if(String.IsNullOrEmpty(encoded))
            {
                return null;
            }
            List<BasicGeoposition> coordsList = new List<BasicGeoposition>();
            char[] polylineChars = encoded.ToArray();
            int index = 0;

            int currentLat = 0;
            int currentLon = 0;
            int next5Bits;
            int sum;
            int shifter;

            try
            {
                while(index < polylineChars.Length)
                {
                    //calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5Bits = (int)polylineChars[index++] - 63;
                        sum |= (next5Bits & 31) << shifter;
                        shifter += 5;
                    } while (next5Bits >= 32 && index < polylineChars.Length);

                    if(index >= polylineChars.Length)
                    {
                        break;
                    }

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1); //~ is the bitwise complement operator

                    //calculate the next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5Bits = (int)polylineChars[index++] - 63;
                        sum |= (next5Bits & 31) << shifter;
                        shifter += 5;
                    } while (next5Bits >= 32 && index < polylineChars.Length);

                    if(index >= polylineChars.Length && next5Bits >= 32)
                    {
                        break;
                    }

                    currentLon += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    BasicGeoposition coords = new BasicGeoposition();
                    coords.Altitude = 0.0;
                    coords.Latitude = Convert.ToDouble(currentLat) / 100000.0; //1E-5 precision, per Google standard
                    coords.Longitude = Convert.ToDouble(currentLon) / 100000.0; //ditto
                    coordsList.Add(coords);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to convert string {encoded} into lat-lon list:\n {ex.Message}");
            }
            return coordsList;
        }
    }
}
