using Windows.Devices.Geolocation;
using Windows.UI;

namespace DigiTransit10.Models
{
    public class ColoredMapLinePoint
    {
        public BasicGeoposition Coordinates { get; set; }
        public bool IsLineDashed { get; set; }
        public Color LineColor { get; set; }

        public ColoredMapLinePoint()
        {
            Coordinates = new BasicGeoposition();
            LineColor = new Color();
        }

        public ColoredMapLinePoint(BasicGeoposition coords, Color color, bool lineDashed = false)
        {
            Coordinates = coords;
            LineColor = color;
            IsLineDashed = lineDashed;
        }
    }
}
