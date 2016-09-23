using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using static DigiTransit10.Models.ModelEnums;

namespace DigiTransit10.Models
{
    //todo: consider implementing INotifyPropertyChanged here
    public interface IFavorite : IComparable<IFavorite>, INotifyPropertyChanged
    {
        Guid FavoriteId { get; set; }
        string UserChosenName { get; set; }
        string FontIconGlyph { get; set; }
        string IconFontFace { get; set; }
        double IconFontSize { get; set; }
    }    

    public class FavoritePlace : IFavorite, IPlace, IComparable<IPlace>
    {
        private static IPlaceComparer _comparer = new IPlaceComparer();

        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; set; }
        public string UserChosenName { get; set; }
        public string IconFontFace { get; set; }
        public string FontIconGlyph { get; set; }
        public Guid FavoriteId { get; set; }   
        public double IconFontSize { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public PlaceType Type { get; set; }
        public double? Confidence { get; set; }
        public Guid OptionalId { get; set; }
        public BasicGeoposition Coords => new BasicGeoposition { Altitude = 0.0, Latitude = Lat, Longitude = Lon };        

        public int CompareTo(IFavorite other)
        {
            return string.Compare(this.UserChosenName, other.UserChosenName, StringComparison.Ordinal);
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
        public Guid FavoriteId { get; set; }
        public double IconFontSize { get; set; }
        public List<string> RouteGeometryStrings { get; set; }
        public List<FavoriteRoutePlace> RoutePlaces { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public int CompareTo(IFavorite other)
        {
            return string.Compare(this.UserChosenName, other.UserChosenName, StringComparison.Ordinal);
        }
    }

    //Only used for seralization due to its small footprint.
    public struct FavoriteRoutePlace
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
