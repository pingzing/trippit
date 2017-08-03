using System;
using System.Collections.Generic;
using System.Linq;

namespace Trippit.Models
{
    public class ColoredMapLine : List<ColoredMapLinePoint>
    {
        public Guid OptionalId { get; set; }

        /// <summary>
        /// Constructs a ColoredMapLine, and retrieves an OptionalId from its first component ColoredMapLinePoint.
        /// </summary>
        /// <param name="lines"></param>
        public ColoredMapLine(IEnumerable<ColoredMapLinePoint> lines)
        {
            this.AddRange(lines);
            if (lines != null && lines.Any())
            {
                OptionalId = lines.First().OptionalId;
            }
        }

        public ColoredMapLine(IEnumerable<ColoredMapLinePoint> lines, Guid id)
        {
            this.AddRange(lines);
            OptionalId = id;
        }
    }
}
