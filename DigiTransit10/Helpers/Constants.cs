using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Helpers
{
    public static class Constants
    {
        //Misc keys
        public const string WideKey = "Wide";
        public const string NarrowKey = "Narrow";

        //Transit mode names
        public const string BusTransitMode = "BUS";
        public const string TramTransitMode = "TRAM";
        public const string TrainTransitMode = "RAIL";
        public const string MetroTransitMode = "SUBWAY";
        public const string FerryTransitMode = "FERRY";
        public const string BikeTransitMode = "BICYCLE";
        public const string WalkTransitMode = "WALK";

        //Resource brush names
        public const string BusBrushName = "HslBusBlueBrush";
        public const string TramBrushName = "HslTramGreenBrush";
        public const string TrainBrushName = "HslTrainPurpleBrush";
        public const string MetroBrushName = "HslMetroOrangeBrush";
        public const string FerryBrushName = "HslFerryBlueBrush";
        public const string BikeBrushName = "HslBikeGoldenrodBrush";
        public const string WalkBrushName = "HslWalkGrayBrush";

        //Font names
        public const string HslPictoNormalFontName = "HslPictoNormalFont";
        public const string HslPictoFrameFontName = "HslPictoFrameFont";

        //Global dictionary Current Visual State keys
        public const string CurrentMainPageVisualStateKey = "CurrentMainPageVisualState";

        //Visual state names
        public const string VisualStateNarrow = "VisualStateNarrow";
        public const string VisualStateNormal = "VisualStateNormal";
        public const string VisualStateWide = "VisualStateWide";
    }
}
