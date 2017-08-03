using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Trippit.Converters
{
    public class NegateBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
