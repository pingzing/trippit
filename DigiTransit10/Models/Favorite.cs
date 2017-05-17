using DigiTransit10.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Geolocation;
using static DigiTransit10.Models.ModelEnums;

namespace DigiTransit10.Models
{    
    public interface IFavorite : IComparable<IFavorite>, INotifyPropertyChanged
    {
        Guid Id { get; set; }
        string UserChosenName { get; set; }
        string FontIconGlyph { get; set; }
        string IconFontFace { get; set; }
        double IconFontSize { get; set; }
    }    

    public class FavoritePlace : IFavorite, IPlace, IComparable<IPlace>, IEquatable<IPlace>
    {
        private static IPlaceComparer _comparer = new IPlaceComparer();

        public event PropertyChangedEventHandler PropertyChanged;

        public string StringId { get; set; }
        private string _userChosenName;
        public string UserChosenName
        {
            get { return _userChosenName; }
            set
            {
                if(_userChosenName != value)
                {
                    _userChosenName = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _iconFontFace;
        public string IconFontFace
        {
            get { return _iconFontFace; }
            set
            {
                if(_iconFontFace != value)
                {
                    _iconFontFace = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _fontIconGlyph;
        public string FontIconGlyph
        {
            get { return _fontIconGlyph; }
            set
            {
                if(_fontIconGlyph != value)
                {
                    _fontIconGlyph = value;
                    RaisePropertyChanged();
                }
            }
        }
        public Guid Id { get; set; }   
        public double IconFontSize { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public PlaceType Type { get; set; }
        public double? Confidence { get; set; }        
        public BasicGeoposition Coords => new BasicGeoposition { Altitude = 0.0, Latitude = Lat, Longitude = Lon };        

        private void RaisePropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int CompareTo(IFavorite other)
        {
            return string.Compare(this.UserChosenName, other.UserChosenName, StringComparison.Ordinal);
        }

        public int CompareTo(IPlace other)
        {
            return _comparer.Compare(this, other);
        }

        public bool Equals(IPlace other)
        {
            FavoritePlace otherPlace = other as FavoritePlace;
            if (otherPlace == null)
            {
                return false;
            }
            return this.Equals(otherPlace);
        }

        public override bool Equals(object obj)
        {
            FavoritePlace other = obj as FavoritePlace;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        protected bool Equals(FavoritePlace other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            return this.Id == other.Id
                && this.Name == other.Name
                && this.Lat == other.Lat
                && this.Lon == other.Lon
                && this.Type == other.Type;
        }

        public static bool operator ==(FavoritePlace a, FavoritePlace b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator != (FavoritePlace a, FavoritePlace b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Lat.GetHashCode();
                hashCode = (hashCode * 397) ^ Lon.GetHashCode();
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                return hashCode;
            }
        }

    }

    public class FavoriteRoute : IFavorite
    {
        private string _userChosenName;
        public string UserChosenName
        {
            get { return _userChosenName; }
            set
            {
                if (_userChosenName != value)
                {
                    _userChosenName = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _iconFontFace;
        public string IconFontFace
        {
            get { return _iconFontFace; }
            set
            {
                if (_iconFontFace != value)
                {
                    _iconFontFace = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _fontIconGlyph;
        public string FontIconGlyph
        {
            get { return _fontIconGlyph; }
            set
            {
                if (_fontIconGlyph != value)
                {
                    _fontIconGlyph = value;
                    RaisePropertyChanged();
                }
            }
        }
        public Guid Id { get; set; }
        public double IconFontSize { get; set; }
        public List<string> RouteGeometryStrings { get; set; }
        public List<SimpleFavoritePlace> RoutePlaces { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int CompareTo(IFavorite other)
        {
            return string.Compare(this.UserChosenName, other.UserChosenName, StringComparison.Ordinal);
        }
    }

    //Only used for seralization due to its small footprint.
    public struct SimpleFavoritePlace : IPlace
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        /// <summary>
        /// Optional.
        /// </summary>
        public string StringId { get; set; }
        
        public PlaceType Type { get; set; }

        /// <summary>
        /// Always returns null on this type.
        /// </summary>
        public double? Confidence
        {
            get { return null; }
            set { }
        }

        public BasicGeoposition Coords => BasicGeopositionExtensions.Create(0.0, Lon, Lat);

        /// <summary>
        /// Always returns Guid.Empty on this type.
        /// </summary>
        public Guid Id
        {
            get { return Guid.Empty; }
            set { }
        }

        public int CompareTo(IPlace other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}
