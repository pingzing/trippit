using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Trippit.Converters
{
    public class SecondsSinceDayStartToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is int))
            {
                return DependencyProperty.UnsetValue;
            }

            int secondsSinceStart = (int) value;
            CultureInfo currCulture = CultureInfo.CurrentUICulture;
            return new DateTime()
                .AddSeconds(secondsSinceStart)
                .ToLocalTime()
                .ToString("t", currCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}