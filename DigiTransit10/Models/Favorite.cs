using DigiTransit10.Localization.Strings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
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

    /// <summary>
    /// A base class for classes that implement IFavorite.
    /// </summary>
    public abstract class FavoriteBase : IFavorite
    {        
        public abstract Guid FavoriteId { get; set; }
        public abstract string UserChosenName { get; set; }
        public abstract string FontIconGlyph { get; set; }
        public abstract string IconFontFace { get; set; }
        public abstract double IconFontSize { get; set; }

        public abstract int CompareTo(IFavorite other);

        /// <summary>
        /// Property for defining a Favorite ID on a child object, such as a MapPolyline, which cannot otherwise have an owner associated with it.
        /// </summary>
        [JsonIgnore]
        public static readonly DependencyProperty FavoriteIdProperty = DependencyProperty.RegisterAttached(
            "FavoriteId",
            typeof(Guid),
            typeof(FavoriteBase),
            new PropertyMetadata(Guid.Empty));

        public event PropertyChangedEventHandler PropertyChanged;

        public static void SetFavoriteId(DependencyObject element, Guid id)
        {
            element.SetValue(FavoriteIdProperty, id);
        }

        public static Guid GetFavoriteId(DependencyObject element)
        {
            return (Guid)element.GetValue(FavoriteIdProperty);
        }        
    }

    public class FavoritePlace : FavoriteBase, IFavorite, IPlace, IComparable<IPlace>
    {
        private static IPlaceComparer _comparer = new IPlaceComparer();

        public string Id { get; set; }
        public override string UserChosenName { get; set; }
        public override string IconFontFace { get; set; }
        public override string FontIconGlyph { get; set; }
        public override Guid FavoriteId { get; set; }   
        public override double IconFontSize { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public PlaceType Type { get; set; }
        public double? Confidence { get; set; }
        public BasicGeoposition Coords => new BasicGeoposition { Altitude = 0.0, Latitude = Lat, Longitude = Lon };        

        public override int CompareTo(IFavorite other)
        {
            return string.Compare(this.UserChosenName, other.UserChosenName, StringComparison.Ordinal);
        }

        public int CompareTo(IPlace other)
        {
            return _comparer.Compare(this, other);
        }
    }

    public class FavoriteRoute : FavoriteBase, IFavorite
    {
        public override string UserChosenName { get; set; }
        public override string FontIconGlyph { get; set; }
        public override string IconFontFace { get; set; }
        public override Guid FavoriteId { get; set; }
        public override double IconFontSize { get; set; }
        public List<string> RouteGeometryStrings { get; set; }
        public List<FavoriteRoutePlace> RoutePlaces { get; set; }        

        public override int CompareTo(IFavorite other)
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
