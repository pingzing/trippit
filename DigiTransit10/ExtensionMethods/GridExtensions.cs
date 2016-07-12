using Windows.UI.Xaml.Controls;

namespace DigiTransit10.ExtensionMethods
{
    public static class GridExtensions
    {
        public static object GetNthGridChildOrNull(this Grid grid, int n)
        {
            if (n <= grid.Children.Count - 1)
            {
                return grid.Children[n];
            }
            else
            {
                return null;
            }
        }
    }
}
