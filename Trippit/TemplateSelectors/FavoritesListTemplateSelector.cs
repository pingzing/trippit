using Trippit.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trippit.TemplateSelectors
{
    public class FavoritesListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FavoritePlaceTemplate { get; set; }
        public DataTemplate FavoriteRouteTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var place = item as FavoritePlace;
            if(place != null)
            {
                return FavoritePlaceTemplate;
            }

            var route = item as FavoriteRoute;
            if(route != null)
            {
                return FavoriteRouteTemplate;
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
