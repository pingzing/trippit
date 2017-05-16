using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Models
{
    public enum WalkingAmountType
    {
        BareMinimum,
        Some,
        Normal,
        Lots,
        Maximum
    }

    public class WalkingAmount
    {
        public WalkingAmountType AmountType { get; set; }
        public string DisplayName { get; set; }
        public float UnderlyingWalkReluctance { get; set; }
    }
}
