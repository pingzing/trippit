using System.Collections.Generic;
using System.Collections.Immutable;
using Trippit.Localization.Strings;
using Trippit.Models;

namespace Trippit.Helpers
{
    public static class Constants
    {
        /// <summary>
        /// Any search results with a confidence lower than this will be filtered out.
        /// </summary>
        public const double SearchResultsMinimumConfidence = 0.35;

        //Font faces
        public const string SegoeMdl2FontName = "Segoe MDL2 Assets";
        public const string SymbolThemeFontResource = "SymbolThemeFontFamily";

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

        //FontFamily keys
        public const string HslPiktoNormalFontFamilyKey = "HslPiktoNormalFont";
        public const string HslPiktoFrameFontFamilyKey = "HslPiktoFrameFont";

        //Font names as they appear in the .ttf file
        public const string HslPiktoNormalFontName = "HSL-Pikto";
        public const string HslPiktoFrameFontName = "HSL-Pikto Frame";

        //Visual state names
        public const string VisualStateNarrow = "VisualStateNarrow";
        public const string VisualStateNormal = "VisualStateNormal";
        public const string VisualStateWide = "VisualStateWide";

        //Font sizes
        public const double SymbolFontSize = 20;
        public const double HslFontSize = 28;

        //Walking amounts and speeds
        public static readonly ImmutableList<WalkingSpeed> WalkingSpeeds = new List<WalkingSpeed>
        {
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Glacial,
                DisplayName = AppResources.WalkingSpeed_0,
                DisplaySubtitle = AppResources.WalkingSpeed_0_Subtitle,
                UnderlyingMetersPerSecond = 0.5f
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Slow,
                DisplayName = AppResources.WalkingSpeed_1,
                DisplaySubtitle = AppResources.WalkingSpeed_1_Subtitle,
                UnderlyingMetersPerSecond = 0.83f
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Normal,
                DisplayName = AppResources.WalkingSpeed_2,
                DisplaySubtitle = AppResources.WalkingSpeed_2_Subtitle,
                UnderlyingMetersPerSecond = 1.2f
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Fast,
                DisplayName = AppResources.WalkingSpeed_3,
                DisplaySubtitle = AppResources.WalkingSpeed_3_Subtitle,
                UnderlyingMetersPerSecond = 2.083f
            },
            new WalkingSpeed
            {
                SpeedType = WalkingSpeedType.Breakneck,
                DisplayName = AppResources.WalkingSpeed_4,
                DisplaySubtitle = AppResources.WalkingSpeed_4_Subtitle,
                UnderlyingMetersPerSecond = 3
            }
        }.ToImmutableList();

        public static WalkingSpeed DefaultWalkingSpeed => WalkingSpeeds[2];

        public static readonly ImmutableList<WalkingAmount> WalkingAmounts = new List<WalkingAmount>
        {
            new WalkingAmount
            {
                AmountType = WalkingAmountType.BareMinimum,
                DisplayName = AppResources.WalkingAmount_0,
                UnderlyingWalkReluctance = 6
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Some,
                DisplayName = AppResources.WalkingAmount_1,
                UnderlyingWalkReluctance = 3.6f
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Normal,
                DisplayName = AppResources.WalkingAmount_2,
                UnderlyingWalkReluctance = 2
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Lots,
                DisplayName = AppResources.WalkingAmount_3,
                UnderlyingWalkReluctance = 1.4f
            },
            new WalkingAmount
            {
                AmountType = WalkingAmountType.Maximum,
                DisplayName = AppResources.WalkingAmount_4,
                UnderlyingWalkReluctance = 0.8f
            }
        }.ToImmutableList();

        public static WalkingAmount DefaultWalkingAmount => WalkingAmounts[2];
    }
}
