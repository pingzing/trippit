using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public PlaceType Type { get; set; }
        public double? Confidence { get; set; }

        public override string ToString()
        {
            return Name;
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
}
