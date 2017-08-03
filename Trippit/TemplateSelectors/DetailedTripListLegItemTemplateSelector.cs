using System;
using Trippit.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trippit.TemplateSelectors
{
    public class DetailedTripListLegItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StartOrMiddleLegTemplate { get; set; }
        public DataTemplate EndLegTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            TripLeg leg = item as TripLeg;
            if(leg == null)
            {
                throw new ArgumentException($"{nameof(DetailedTripListLegItemTemplateSelector)} expects only items of type {nameof(TripLeg)}");
            }

            if(leg.IsEnd)
            {
                return EndLegTemplate;
            }
            else
            {
                return StartOrMiddleLegTemplate;
            }
        }
    }
}
