using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DigiTransit10.Converters
{
    public class ElementThemeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is ElementTheme))
            {
                return null;
            }

            ElementTheme theme = (ElementTheme)value;
            return Enum.GetName(typeof(ElementTheme), theme);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return ElementTheme.Default;
            }

            ElementTheme result;
            if (Enum.TryParse<ElementTheme>(value as string, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
