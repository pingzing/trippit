using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class CircleMapIconSource : UserControl
    {
        private static bool _initialized = false;
        private static Grid _topmostGrid = null;
        private static CircleMapIconSource _source;

        private static IRandomAccessStream _themeColoredBitmap = null;
        private static IRandomAccessStream _greyedOutBitmap = null;

        private static void Initialize()
        {
            _topmostGrid = Template10.Utils.XamlUtils.FirstChild<Grid>(Window.Current.Content);
            _source = new CircleMapIconSource();
            _source.RenderTransform = new CompositeTransform { TranslateX = -500 };
            _topmostGrid.Children.Add(_source);
            _initialized = true;
            _topmostGrid.UpdateLayout();
        }        

        public static async Task<IRandomAccessStream> GenerateThemeColorAsync()
        {
            if (!_initialized)
            {
                Initialize();
            }
                        
            if (_themeColoredBitmap != null)
            {
                return _themeColoredBitmap;
            }
            else
            {                
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(_source.ThemeColoredCircle);
                _themeColoredBitmap = (await rtb.GetPixelsAsync()).AsStream().AsRandomAccessStream();
                return _themeColoredBitmap;
            }
        }

        public static async Task<IRandomAccessStream> GenerateGreyedOutAsync()
        {            
            if (!_initialized)
            {
                Initialize();
            }

            if (_greyedOutBitmap != null)
            {
                return _greyedOutBitmap;
            }
            else
            {                
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(_source.GreyedOutCircle);
                _greyedOutBitmap = (await rtb.GetPixelsAsync()).AsStream().AsRandomAccessStream();
                return _greyedOutBitmap;
            }
        }

        public CircleMapIconSource()
        {
            this.InitializeComponent();
        }
    }
}
