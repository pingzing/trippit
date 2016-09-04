using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Helpers
{
    public static class Constants
    {
        //Font faces
        public const string SymbolFontFamily = "Segoe MDL2 Assets";

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

        //Color names
        public const string BusColorName = "HslBusBlue";
        public const string TramColorName = "HslTramGreen";
        public const string TrainColorName = "HslTrainPurple";
        public const string MetroColorName = "HslMetroOrange";
        public const string FerryColorName = "HslFerryBlue";
        public const string BikeColorName = "HslBikeGoldenrod";
        public const string WalkColorName = "HslWalkGray";

        //Resource brush names
        public const string BusBrushName = "HslBusBlueBrush";
        public const string TramBrushName = "HslTramGreenBrush";
        public const string TrainBrushName = "HslTrainPurpleBrush";
        public const string MetroBrushName = "HslMetroOrangeBrush";
        public const string FerryBrushName = "HslFerryBlueBrush";
        public const string BikeBrushName = "HslBikeGoldenrodBrush";
        public const string WalkBrushName = "HslWalkGrayBrush";

        //Other misc Resource names
        public const string TallTextBoxHeightName = "TallTextBoxHeight";
        public const string TallTextBoxFontSizeName = "TallTextBoxFontSize";

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
