using System;
using Trippit.Localization.Strings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Trippit.Converters
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
            switch (theme)
            {
                case ElementTheme.Dark:
                    return AppResources.DarkThemeName;
                case ElementTheme.Light:
                    return AppResources.LightThemeName;
                case ElementTheme.Default:
                default:
                    return AppResources.SystemSettingThemeName;                    
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            string stringTheme = (value as ComboBoxItem)?.Content as string;
            if (stringTheme == null)
            {
                stringTheme = value as string;
            }
            if (stringTheme == null)
            {
                return ElementTheme.Default;
            }
            if (stringTheme == AppResources.DarkThemeName)
            {
                return ElementTheme.Dark;
            }
            else if (stringTheme == AppResources.LightThemeName)
            {
                return ElementTheme.Light;
            }
            else
            {
                return ElementTheme.Default;
            }
        }
    }
}
