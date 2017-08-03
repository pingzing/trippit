using System;
using Windows.UI.Xaml.Data;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Converters
{
    public class TransitModeToConditionalAccentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is ApiMode))
            {
                return false;
            }
            ApiMode mode = (ApiMode)value;
            if (mode == ApiMode.Bus
                || mode == ApiMode.Rail
                || mode == ApiMode.Subway
                || mode == ApiMode.Tram
                || mode == ApiMode.Ferry)
            {
                return App.Current.Resources["SystemControlBackgroundAccentBrush"];
            }
            else
            {
                return App.Current.Resources["SystemControlForegroundBaseHighBrush"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
