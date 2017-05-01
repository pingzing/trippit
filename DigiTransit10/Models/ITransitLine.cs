using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models
{
    public interface ITransitLine
    {
        ApiMode TransitMode { get; set; }
        string GtfsId { get; set; }
        string ShortName { get; set; }
        string LongName { get; set; }
    }
}
