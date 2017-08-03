using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trippit.Controls
{
    public sealed partial class MapSelfMarker : UserControl
    {
        public static int MapSelfMarkerZIndex = 999;

        public const double RenderTransformOriginX = .38;
        public const double RenderTransformOriginY = .5;

        public static readonly DependencyProperty RotationDegreesProperty =
            DependencyProperty.Register("RotationDegrees", typeof(double), typeof(MapSelfMarker), new PropertyMetadata(0,
                RotationDegreesChanged));
        private static void RotationDegreesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as MapSelfMarker;
            if(_this == null)
            {
                return;
            }

            double newRotation = (double)e.NewValue - 90; //subtract 90 to account for the fact that the arrow faces right, and not up                        
            _this.RotationTransform.Angle = newRotation;
        }
        public double RotationDegrees
        {
            get { return (double)GetValue(RotationDegreesProperty); }
            set { SetValue(RotationDegreesProperty, value); }
        }

        public static readonly DependencyProperty IsArrowVisibleProperty =
            DependencyProperty.Register("IsArrowVisible", typeof(bool), typeof(MapSelfMarker), new PropertyMetadata(false,
                IsArrowVisibleChanged));
        private static void IsArrowVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as MapSelfMarker;
            if (_this == null)
            {
                return;
            }

            bool newIsVisible = (bool)e.NewValue;
            if(newIsVisible)
            {
                _this.ArrowPath.Visibility = Visibility.Visible;
            }
            else
            {
                _this.ArrowPath.Visibility = Visibility.Collapsed;
            }
        }
        public bool IsArrowVisible
        {
            get { return (bool)GetValue(IsArrowVisibleProperty); }
            set { SetValue(IsArrowVisibleProperty, value); }
        }

        public MapSelfMarker()
        {
            this.InitializeComponent();
        }
    }
}
