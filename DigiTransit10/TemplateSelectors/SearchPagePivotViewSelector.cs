using DigiTransit10.ViewModels.ControlViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static DigiTransit10.ViewModels.ControlViewModels.StopSearchContentViewModel;

namespace DigiTransit10.TemplateSelectors
{
    public class SearchPagePivotViewSelector : DataTemplateSelector
    {
        public DataTemplate NearbyStopsSearchTemplate { get; set; }
        public DataTemplate LinesSearchTemplate { get; set; }
        public DataTemplate StopsSearchTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var stopSearchVm = item as StopSearchContentViewModel;
            if (stopSearchVm != null)
            {
                return stopSearchVm.OwnedBy == OwnerSearchPivot.NearbyStops ? NearbyStopsSearchTemplate
                    : stopSearchVm.OwnedBy == OwnerSearchPivot.Stops ? StopsSearchTemplate
                    : null;
            }

            var linesSearchVm = item as LineSearchContentViewModel;
            if (linesSearchVm != null)
            {
                return LinesSearchTemplate;
            }

            return null;
        }
    }
}
