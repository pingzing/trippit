using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using static DigiTransit10.Models.ModelEnums;

namespace DigiTransit10.Models
{
    public interface IFavorite : IComparable<IFavorite>
    {
        string UserChosenName { get; set; }
        string FontIconGlyph { get; set; }
        string IconFontFace { get; set; }
    }

    public class FavoritePlace : IFavorite, IPlace, IComparable<IPlace>
    {
        private static IPlaceComparer _comparer = new IPlaceComparer();

        public string Id { get; set; }
        public string UserChosenName { get; set; }
        public string IconFontFace { get; set; }
        public string FontIconGlyph { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public PlaceType Type { get; set; }
        public double? Confidence { get; set; }
        public BasicGeoposition Coords => new BasicGeoposition { Altitude = 0.0, Latitude = Lat, Longitude = Lon };

        public int CompareTo(IFavorite other)
        {
            return this.UserChosenName.CompareTo(other.UserChosenName);
        }

        public int CompareTo(IPlace other)
        {
            return _comparer.Compare(this, other);
        }
    }

    public class FavoriteRoute : IFavorite
    {
        public string UserChosenName { get; set; }
        public string FontIconGlyph { get; set; }
        public string IconFontFace { get; set; }
        public List<Place> RoutePlaces { get; set; }

        public int CompareTo(IFavorite other)
        {
            return this.UserChosenName.CompareTo(other.UserChosenName);
        }
    }
}
