using DigiTransit10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.TemplateSelectors
{    
    public class PlaceItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AddressTemplate { get; set; }
        public DataTemplate StopTemplate { get; set; }
        public DataTemplate CoordinatesTemplate { get; set; }
        public DataTemplate UserCurrentLocationTemplate { get; set; }
        public DataTemplate FavoritePlaceTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            IPlace place = item as IPlace;
            if(place != null)
            {
                if(place.Type == ModelEnums.PlaceType.Address)
                {
                    return AddressTemplate;
                }
                else if(place.Type == ModelEnums.PlaceType.Stop)
                {
                    return StopTemplate;
                }
                else if(place.Type == ModelEnums.PlaceType.Coordinates)
                {
                    return CoordinatesTemplate;
                }
                else if(place.Type == ModelEnums.PlaceType.UserCurrentLocation)
                {
                    return UserCurrentLocationTemplate;
                }
                else if(place.Type == ModelEnums.PlaceType.FavoritePlace)
                {
                    return FavoritePlaceTemplate;
                }
            }

            return base.SelectTemplateCore(item, container);
        }

    }
}
