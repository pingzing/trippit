using DigiTransit10.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Converters
{
    public class TransitModeToForegroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is ApiMode))
            {
                return DependencyProperty.UnsetValue;
            }

            ApiMode mode = (ApiMode)value;
            switch (mode)
            {
                case ApiMode.Bicycle:
                    return Application.Current.Resources[Constants.BikeBrushName];
                case ApiMode.Bus:
                    return Application.Current.Resources[Constants.BusBrushName];
                case ApiMode.Ferry:
                    return Application.Current.Resources[Constants.FerryBrushName];
                case ApiMode.Rail:
                    return Application.Current.Resources[Constants.TrainBrushName];
                case ApiMode.Subway:
                    return Application.Current.Resources[Constants.MetroBrushName];
                case ApiMode.Tram:
                    return Application.Current.Resources[Constants.TramBrushName];
                case ApiMode.Walk:
                    return Application.Current.Resources[Constants.WalkBrushName];
                default:
                    return Application.Current.Resources[Constants.WalkBrushName];
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
