using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using DigiTransit10.Models.ApiModels;
using static DigiTransit10.Models.ApiModels.ApiEnums;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripDetailListTransitIcon : UserControl
    {
        public static readonly DependencyProperty TripLegProperty = DependencyProperty.Register(
            "TripLeg", typeof (ApiLeg), typeof (TripDetailListTransitIcon), new PropertyMetadata(null,
                TripLegChanged));
        private static void TripLegChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripDetailListTransitIcon _this = d as TripDetailListTransitIcon;
            if (_this == null)
            {
                return;
            }
            ApiLeg newLeg = (ApiLeg) e.NewValue;

            switch (newLeg.Mode)
            {
                case ApiMode.Bicycle:
                    _this.TransitIconTopTextBlock.Text = newLeg.Route.ShortName;
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.BikeBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.BikeIcon;
                    break;
                case ApiMode.Bus:
                    _this.TransitIconTopTextBlock.Text = newLeg.Route.ShortName;
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.BusBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.BusIcon;
                    break;
                case ApiMode.Ferry:
                    _this.TransitIconTopTextBlock.Text = newLeg.Route.ShortName;
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.FerryBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.FerryIcon;
                    break;
                case ApiMode.Rail:
                    _this.TransitIconTopTextBlock.Text = newLeg.Route.ShortName;
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.TrainBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.TrainIcon;
                    break;
                case ApiMode.Subway:                    
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.MetroBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.MetroIcon;
                    break;
                case ApiMode.Tram:
                    _this.TransitIconTopTextBlock.Text = newLeg.Route.ShortName;
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.TramBrushName];
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.TramIcon;
                    break;                
                case ApiMode.Walk:
                    _this.TransitIconTopTextBlock.Text = newLeg.Distance + "m";
                    _this.TransitModeIcon.Glyph = FontIconGlyphs.WalkIcon;
                    _this.TransitModeIcon.Foreground = (Brush)Application.Current.Resources[Constants.WalkBrushName];
                    break;                    
            }
            if (String.IsNullOrWhiteSpace(_this.TransitIconTopTextBlock.Text))
            {
                _this.TransitIconTopTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        public ApiLeg TripLeg
        {
            get { return (ApiLeg) GetValue(TripLegProperty); }
            set { SetValue(TripLegProperty, value); }
        }

        public TripDetailListTransitIcon()
        {
            this.InitializeComponent();
        }
    }
}
