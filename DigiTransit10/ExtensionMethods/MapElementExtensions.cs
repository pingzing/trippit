using Newtonsoft.Json;
using System;
using Windows.UI.Xaml;

namespace DigiTransit10.ExtensionMethods
{
    public static class MapElementExtensions
    {
        /// <summary>
        /// Property for defining an ID on any kind of POI, such as a MapElement, which cannot otherwise have an owner associated with it.
        /// </summary>
        [JsonIgnore]
        public static readonly DependencyProperty PoiId = DependencyProperty.RegisterAttached(
            "PoiId",
            typeof(Guid),
            typeof(MapElementExtensions),
            new PropertyMetadata(Guid.Empty));

        public static void SetPoiId(DependencyObject element, Guid id)
        {
            element.SetValue(PoiId, id);
        }

        public static Guid GetPoiId(DependencyObject element)
        {
            return (Guid)element.GetValue(PoiId);
        }       
    }
}
