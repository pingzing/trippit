using Trippit.Helpers;
using Windows.UI;
using Windows.UI.Xaml;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Styles
{
    public static class HslColors
    {
        public static Color GetModeColor(ApiMode mode)
        {
            switch (mode)
            {
                case ApiMode.Bicycle:
                    return (Color)Application.Current.Resources[Constants.BikeColorName];
                case ApiMode.Bus:
                    return (Color)Application.Current.Resources[Constants.BusColorName];
                case ApiMode.Ferry:
                    return (Color)Application.Current.Resources[Constants.FerryColorName];
                case ApiMode.Rail:
                    return (Color)Application.Current.Resources[Constants.TrainColorName];
                case ApiMode.Subway:
                    return (Color)Application.Current.Resources[Constants.MetroColorName];
                case ApiMode.Tram:
                    return (Color)Application.Current.Resources[Constants.TramColorName];
                case ApiMode.Walk:
                    return (Color)Application.Current.Resources[Constants.WalkColorName];
                default:
                    return Colors.White;
            }
        }
    }
}
