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

    public class Place : IPlace
    {
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public PlaceType Type { get; set; }
    }
}
