using System;
using Windows.Devices.Geolocation;
using Windows.UI;

namespace Trippit.Models
{
    public class ColoredMapLinePoint
    {
        public BasicGeoposition Coordinates { get; set; }
        public bool IsLineDashed { get; set; }
        public Color LineColor { get; set; }
        public Guid OptionalId { get; set; }

        public ColoredMapLinePoint()
        {
            Coordinates = new BasicGeoposition();
            LineColor = new Color();
        }

        public ColoredMapLinePoint(Guid id)
        {
            Coordinates = new BasicGeoposition();
            LineColor = new Color();
            OptionalId = id;
        }

        public ColoredMapLinePoint(BasicGeoposition coords, Color color, bool lineDashed = false)
        {
            Coordinates = coords;
            LineColor = color;
            IsLineDashed = lineDashed;
        }

        public ColoredMapLinePoint(BasicGeoposition coords, Color color, Guid id, bool lineDashed = false)
        {
            Coordinates = coords;
            LineColor = color;
            IsLineDashed = lineDashed;
            OptionalId = id;
        }
    }
}
