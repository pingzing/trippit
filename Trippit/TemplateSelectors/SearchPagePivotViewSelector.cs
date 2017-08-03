using Trippit.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trippit.TemplateSelectors
{
    public class SearchPagePivotViewSelector : DataTemplateSelector
    {
        public DataTemplate NearbyStopsSearchTemplate { get; set; }
        public DataTemplate LinesSearchTemplate { get; set; }
        public DataTemplate StopsSearchTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var vm = item as ISearchViewModel;
            if (vm != null)
            {
                switch (vm.OwnedBy)
                {
                    case ViewModels.SearchSection.Nearby:
                        return NearbyStopsSearchTemplate;
                    case ViewModels.SearchSection.Lines:
                        return LinesSearchTemplate;
                    case ViewModels.SearchSection.Stops:
                        return StopsSearchTemplate;
                    default:
                        return null;
                }
            }

            return null;
        }
    }
}
