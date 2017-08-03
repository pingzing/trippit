using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Controls
{
    public sealed partial class CircleMapIconSource : UserControl
    {
        private static bool _initialized = false;
        private static Grid _topmostGrid = null;
        private static CircleMapIconSource _source;

        private static IRandomAccessStream _themeColoredBitmap = null;
        private static IRandomAccessStream _themeColorPointerOverBitmap = null;
        private static IRandomAccessStream _themeColoredSelectedBitmap = null;
        private static IRandomAccessStream _greyedOutBitmap = null;        

        public enum IconType
        {
            GreyedOut,
            ThemeColor,
            ThemeColorPointerOver,
            ThemeColorSelected
        }

        // Make sure to add any new IconTypes here.
        private static readonly Dictionary<IconType, IRandomAccessStream> TypeToBufferMappings =
            new Dictionary<IconType, IRandomAccessStream>
            {
                { IconType.GreyedOut, _greyedOutBitmap },
                { IconType.ThemeColor, _themeColoredBitmap },
                { IconType.ThemeColorPointerOver, _themeColorPointerOverBitmap },
                { IconType.ThemeColorSelected, _themeColoredSelectedBitmap },
            };

        private static readonly Dictionary<IconType, UIElement> TypeToXamlMappings = 
            new Dictionary<IconType, UIElement>(TypeToBufferMappings.Keys.Count);

        private static void Initialize()
        {
            _topmostGrid = Window.Current.Content.FirstChild<Grid>();
            _source = new CircleMapIconSource();
            _source.RenderTransform = new CompositeTransform { TranslateX = -500, TranslateY = -500 };

            // Make sure to add any new IconTypes here.
            TypeToXamlMappings.Add(IconType.GreyedOut, _source.GreyedOutCircle);
            TypeToXamlMappings.Add(IconType.ThemeColor, _source.ThemeColoredCircle);
            TypeToXamlMappings.Add(IconType.ThemeColorPointerOver, _source.ThemeColoredPointerOverCircle);
            TypeToXamlMappings.Add(IconType.ThemeColorSelected, _source.ThemeColoredSelectedCircle);

            DispatcherHelper.CheckBeginInvokeOnUI(() => 
            {
                _topmostGrid.Children.Add(_source);
                _topmostGrid.UpdateLayout();
                _initialized = true;
            });                                    
        }        

        public static async Task<IRandomAccessStream> GenerateIconAsync(IconType iconType)
        {
            if (!_initialized)
            {
                Initialize();
            }

            IRandomAccessStream streamToReturn = TypeToBufferMappings[iconType];
            if (streamToReturn == null)
            {
                UIElement elementToRender = TypeToXamlMappings[iconType];
                streamToReturn = await ToRandomAccessStream(elementToRender);
                return streamToReturn;
            }
            else
            {
                return streamToReturn;
            }
        }        
        
        public CircleMapIconSource()
        {
            this.InitializeComponent();
        }

        private static async Task<IRandomAccessStream> ToRandomAccessStream(UIElement element)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(element);
            IBuffer pixelBuffer = await rtb.GetPixelsAsync();
            byte[] pixels = pixelBuffer.ToArray();

            DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();

            var stream = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                (uint)rtb.PixelWidth,
                (uint)rtb.PixelHeight,
                displayInfo.RawDpiX,
                displayInfo.RawDpiY,
                pixels);

            await encoder.FlushAsync();
            stream.Seek(0);

            return stream;
        }
    }
}
