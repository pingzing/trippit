using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Models
{
    public static class ModelEnums
    {
        public enum PlaceType
        {
            UserCurrentLocation,
            Stop,
            Address,
            Coordinates,
            NameOnly,            
        }
    }
}
