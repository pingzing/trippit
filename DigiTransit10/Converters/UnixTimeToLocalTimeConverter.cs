using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DigiTransit10.Converters
{
    public class UnixTimeToLocalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is long))
            {
                return DependencyProperty.UnsetValue;
            }

            long unixTime = (long)value;

            CultureInfo currCulture = CultureInfo.CurrentUICulture;
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTime)
                .LocalDateTime
                .ToString("t", currCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
