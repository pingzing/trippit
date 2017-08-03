using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Trippit.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is bool)
            {
                bool bVal = (bool)value;
                return bVal ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (value is bool?)
            {
                bool? bval = (bool?)value;
                if (bval == null || bval.Value == false)
                {
                    return Visibility.Collapsed;
                }                
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value is Visibility)
            {
                Visibility vis = (Visibility)value;
                return vis == Visibility.Visible ? true : false;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
