using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Trippit.Converters
{
    public class ListViewSelectionModeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is ListViewSelectionMode))
            {
                return null;
            }

            var currentMode = (ListViewSelectionMode)value;
            if(currentMode == ListViewSelectionMode.None)
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
            if(!(value is bool))
            {
                return false;
            }

            var currMode = (bool)value;
            if(currMode)
            {
                return ListViewSelectionMode.Multiple;
            }
            else
            {
                return ListViewSelectionMode.None;
            }
        }
    }
}
