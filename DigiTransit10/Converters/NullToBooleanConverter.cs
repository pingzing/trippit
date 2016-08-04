using System;
using Windows.UI.Xaml.Data;

namespace DigiTransit10.Converters
{
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Cannot convert back to a nullable object from a boolean.");
        }
    }
}
