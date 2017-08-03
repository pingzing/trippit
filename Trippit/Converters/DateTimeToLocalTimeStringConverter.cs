using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Trippit.Converters
{
    public class DateTimeToLocalTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is DateTime))
            {
                return DependencyProperty.UnsetValue;
            }

            var dateTime = (DateTime)value;

            CultureInfo currCulture = CultureInfo.CurrentUICulture;
            return dateTime.ToLocalTime().ToString("t", currCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
