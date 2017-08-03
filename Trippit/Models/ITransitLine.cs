using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Models
{
    public interface ITransitLine
    {
        ApiMode TransitMode { get; set; }
        string GtfsId { get; set; }
        string ShortName { get; set; }
        string LongName { get; set; }
    }
}
