using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigiTransit10.Models.ModelEnums;

namespace DigiTransit10.Models
{
    public interface IFavorite
    {
        string UserChosenName { get; set; }       
        //some kind of icon? 
    }

    public class FavoritePlace : IFavorite, IPlace
    {
        public string UserChosenName { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public PlaceType Type { get; set; }
    }

    public class FavoriteRoute : IFavorite
    {
        public string UserChosenName { get; set; }
        public List<Place> RoutePlaces { get; set; }
    }
}
