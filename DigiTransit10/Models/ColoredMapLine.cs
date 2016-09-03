using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Models
{
    public class ColoredMapLine : List<ColoredMapLinePoint>
    {
        public Guid? FavoriteId { get; set; }

        public ColoredMapLine(IEnumerable<ColoredMapLinePoint> lines)
        {
            this.AddRange(lines);
            FavoriteId = null;
        }

        public ColoredMapLine(IEnumerable<ColoredMapLinePoint> lines, Guid favoriteId)
        {
            this.AddRange(lines);
            FavoriteId = favoriteId;
        }
    }
}
