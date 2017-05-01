using System;
using Windows.UI.Xaml.Data;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Converters
{
    public class IsTransitModeSearchableConverter : IValueConverter
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
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
