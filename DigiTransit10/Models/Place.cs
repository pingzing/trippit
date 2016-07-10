using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static DigiTransit10.Models.ModelEnums;

namespace DigiTransit10.Models
{
    public interface IPlace
    {        
        string Name { get; set; }
        float Lat { get; set; }
        float Lon { get; set; }
        PlaceType Type { get; set; }        
    }

    public class Place : IPlace, IComparable<Place>
    {        
        public string Id { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public PlaceType Type { get; set; }
        public double? Confidence { get; set; }        

        public override bool Equals(object obj)
        {
            Place other = obj as Place;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        protected bool Equals(Place other)
        {
            if(object.ReferenceEquals(other, null))
            {
                return false;
            }
            return this.Id == other.Id
                && this.Name.Equals(other.Name)
                && this.Lat == other.Lat
                && this.Lon == other.Lon
                && this.Type == other.Type
                && this.Confidence == other.Confidence;
        }

        public static bool operator ==(Place a, Place b)
        {
            if(System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object) a == null || (object) b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Place a, Place b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Id?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Lat.GetHashCode();
                hashCode = (hashCode * 397) ^ Lon.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ Confidence.GetHashCode();
                return hashCode;
            }
        }

        public int CompareTo(Place other)
        {
            //this should take into account placetype (rank Stops higher than addresses) and confidence. if no confidence, fall back to alphabetical            
            int scoreSoFar = 0;
            scoreSoFar = this.Type.CompareTo(other.Type);// * -1; //we want descending order, default is ascending
            if (scoreSoFar != 0)
            {
                return scoreSoFar;
            }

            if(this.Confidence != null && other.Confidence != null)
            {
                scoreSoFar = (this.Confidence.Value).CompareTo(other.Confidence.Value) * -1; //we want descending order, default is ascending
            }

            if(scoreSoFar != 0)
            {
                return scoreSoFar;
            }

            return this.Name.CompareTo(other.Name);
        }
    }

    public class PlaceComparer : IComparer<Place>
    {
        public int Compare(Place a, Place b)
        {
            //this should take into account placetype (rank Stops higher than addresses) and confidence. if no confidence, fall back to alphabetical            
            int scoreSoFar = 0;
            scoreSoFar = a.Type.CompareTo(b.Type);
            if (scoreSoFar != 0)
            {
                return scoreSoFar;
            }

            if (a.Confidence != null && b.Confidence != null)
            {
                scoreSoFar = (a.Confidence.Value).CompareTo(b.Confidence.Value) * -1; //we want descending order for score, default is ascending
            }

            if (scoreSoFar != 0)
            {
                return scoreSoFar;
            }

            return a.Name.CompareTo(b.Name);
        }
    }
}
