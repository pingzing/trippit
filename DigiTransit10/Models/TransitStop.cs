using System;
using Windows.Devices.Geolocation;

namespace DigiTransit10.Models
{
    public class TransitStop
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public BasicGeoposition Coords { get; set; }
        public string NameAndCode
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Code))
                {
                    return $"{Name}, {Code}";
                }
                else
                {
                    return Name;
                }
            }
        }
    }
}