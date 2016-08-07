using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using DigiTransit10.Helpers;
using static DigiTransit10.Models.ApiModels.ApiEnums;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls.TripPlanStrip
{
    public sealed partial class TripPlanTransitIcon : UserControl
    {
        public static readonly DependencyProperty TransitModeProperty =
            DependencyProperty.Register("TransitMode", typeof(ApiMode), typeof(TripPlanTransitIcon), new PropertyMetadata(0, 
                TransitModeChanged));
        private static void TransitModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as TripPlanTransitIcon;
            if (_this == null)
            {
                return;
            }

            if (!(e.NewValue is ApiMode))
            {
                return;
            }

            ApiMode newMode = (ApiMode)e.NewValue;
            switch (newMode)
            {
                case ApiMode.Bicycle:
                    _this.TransitModeIcon.Foreground =  (Brush)Application.Current.Resources[Constants.BikeBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.BikeIcon;
                    break;
                case ApiMode.Bus:
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.BusBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.BusIcon;
                    break;
                case ApiMode.Ferry:
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.FerryBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.FerryIcon;
                    break;                
                case ApiMode.Rail:
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.TrainBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.TrainIcon;
                    break;
                case ApiMode.Subway:
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.MetroBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.MetroIcon;
                    break;
                case ApiMode.Tram:
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.TramBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.TramIcon;
                    break;
                case ApiMode.Walk:
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.WalkBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.WalkIcon;
                    _this.TransitModeIcon.FontFamily = (FontFamily)Application.Current.Resources[Constants.HslPictoNormalFontName];                    
                    _this.NameOrDistanceBlock.FontWeight = FontWeights.Normal;
                    break;
            }           
        }
        public ApiMode TransitMode
        {
            get { return (ApiMode)GetValue(TransitModeProperty); }
            set { SetValue(TransitModeProperty, value); }
        }                
        
        public static readonly DependencyProperty ShortNameProperty =
            DependencyProperty.Register("ShortName", typeof(string), typeof(TripPlanTransitIcon), new PropertyMetadata(null, new PropertyChangedCallback(ShortNameChanged)));

        private static void ShortNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripPlanTransitIcon thisControl = d as TripPlanTransitIcon;
            if (e.NewValue != null)
            {
                thisControl.NameOrDistanceBlock.Text = (string)e.NewValue;
            }
            else
            {
                if (thisControl.Distance != null)
                {
                    thisControl.NameOrDistanceBlock.Text = thisControl.Distance.Value.ToString("N0") + "m";
                }
                else
                {
                    thisControl.NameOrDistanceBlock.Text = "???";
                }
            }
        }

        public string ShortName
        {
            get { return (string)GetValue(ShortNameProperty); }
            set { SetValue(ShortNameProperty, value); }
        }
        
        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(float?), typeof(TripPlanTransitIcon), new PropertyMetadata(null, new PropertyChangedCallback(DistanceChanged)));

        private static void DistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripPlanTransitIcon thisControl = d as TripPlanTransitIcon;
            if(thisControl.ShortName != null)
            {
                return;
            }
            else
            {
                if(e.NewValue != null)
                {
                    float distance = (float)e.NewValue;
                    thisControl.NameOrDistanceBlock.Text = distance.ToString("N0") + "m";
                }
                else
                {
                    thisControl.NameOrDistanceBlock.Text = "???";
                }
            }
        }

        public float? Distance
        {
            get { return (float?)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(TripPlanTransitIcon), new PropertyMetadata(0, new PropertyChangedCallback(HorizontalOffsetChanged)));
        private static void HorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripPlanTransitIcon thisControl = d as TripPlanTransitIcon;
            if(d == null)
            {
                return;
            }
            if (e.NewValue is double)
            {
                double newOffset = (double)e.NewValue;
                thisControl.NameOrDistanceBlock.RenderTransform = new TranslateTransform { X = newOffset };
                thisControl.TransitModeIcon.RenderTransform = new TranslateTransform { X = newOffset };
            }
        }
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }              

        public TripPlanTransitIcon()
        {
            this.InitializeComponent();
        }
    }
}
