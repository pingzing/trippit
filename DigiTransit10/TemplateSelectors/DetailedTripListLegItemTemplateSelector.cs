using DigiTransit10.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.TemplateSelectors
{
    public class DetailedTripListLegItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StartOrMiddleLegTemplate { get; set; }
        public DataTemplate EndLegTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            DetailedTripListLeg leg = item as DetailedTripListLeg;
            if(leg == null)
            {
                throw new ArgumentException($"{nameof(DetailedTripListLegItemTemplateSelector)} expects only items of type {nameof(DetailedTripListLeg)}");
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
