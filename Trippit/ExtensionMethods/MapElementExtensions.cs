using Newtonsoft.Json;
using System;
using Trippit.Controls;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace Trippit.ExtensionMethods
{
    public static class MapElementExtensions
    {
        /// <summary>
        /// Property for defining an ID on any kind of POI, such as a MapElement, which cannot otherwise have an owner associated with it.
        /// </summary>
        [JsonIgnore]
        public static readonly DependencyProperty PoiIdProperty = DependencyProperty.RegisterAttached(
            "PoiId",
            typeof(Guid),
            typeof(MapElementExtensions),
            new PropertyMetadata(Guid.Empty));

        public static void SetPoiId(DependencyObject element, Guid id)
        {
            element.SetValue(PoiIdProperty, id);
        }

        public static Guid GetPoiId(DependencyObject element)
        {
            return (Guid)element.GetValue(PoiIdProperty);
        }

        public enum MapIconState
        {
            None,
            PointerOver,
            Selected
        }
        public static readonly DependencyProperty MapIconStateProperty = DependencyProperty.RegisterAttached(
            "MapIconState",
            typeof(MapIconState),
            typeof(MapElementExtensions),
            new PropertyMetadata(MapIconState.None, MapIconStateChanged));

        private static async void MapIconStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapIcon _this = d as MapIcon;
            if (_this == null)
            {
                return;
            }

            MapIconState? oldValue = (MapIconState?)e.OldValue;
            MapIconState? newValue = (MapIconState?)e.NewValue;

            // TODO: Maybe have some logic depending on what oldState was.
            if (newValue == null)
            {
                return;
            }

            IRandomAccessStream stream;
            switch (newValue.Value)
            {
                case MapIconState.PointerOver:
                    stream = await CircleMapIconSource.GenerateIconAsync(CircleMapIconSource.IconType.ThemeColorPointerOver);
                    _this.ZIndex = 999;
                    _this.Image = RandomAccessStreamReference.CreateFromStream(stream);
                    return;
                case MapIconState.Selected:
                    stream = await CircleMapIconSource.GenerateIconAsync(CircleMapIconSource.IconType.ThemeColorSelected);
                    _this.ZIndex = 999;
                    _this.Image = RandomAccessStreamReference.CreateFromStream(stream);
                    return;
                default:
                case MapIconState.None:
                    stream = await CircleMapIconSource.GenerateIconAsync(CircleMapIconSource.IconType.ThemeColor);
                    _this.ZIndex = 1;
                    _this.Image = RandomAccessStreamReference.CreateFromStream(stream);
                    return;
            }
        }

        public static void SetMapIconState(DependencyObject element, MapIconState state)
        {
            element.SetValue(MapIconStateProperty, state);
        }

        public static MapIconState GetMapIconState(DependencyObject element)
        {
            return (MapIconState)element.GetValue(MapIconStateProperty);
        }
    }
}
