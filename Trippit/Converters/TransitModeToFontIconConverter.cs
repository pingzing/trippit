﻿using System;
using Trippit.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.Converters
{
    public class TransitModeToFontIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is ApiMode))
            {
                return DependencyProperty.UnsetValue;
            }

            ApiMode mode = (ApiMode)value;
            switch (mode)
            {
                case ApiMode.Bicycle:
                    return FontIconGlyphs.BikeIcon;
                case ApiMode.Bus:
                    return FontIconGlyphs.BusIcon;
                case ApiMode.Ferry:
                    return FontIconGlyphs.FerryIcon;
                case ApiMode.Rail:
                    return FontIconGlyphs.TrainIcon;
                case ApiMode.Subway:
                    return FontIconGlyphs.MetroIcon;
                case ApiMode.Tram:
                    return FontIconGlyphs.TramIcon;
                case ApiMode.Walk:
                    return FontIconGlyphs.WalkIcon;
                default:
                    return FontIconGlyphs.WalkIcon;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string iconGlyph = value as string;
            if(iconGlyph == null)
            {
                return DependencyProperty.UnsetValue;
            }


            if (iconGlyph == FontIconGlyphs.BikeIcon)
            {
                return ApiMode.Bicycle;
            }
            if (iconGlyph == FontIconGlyphs.BusIcon)
            {
                return ApiMode.Bus;
            }
            if(iconGlyph == FontIconGlyphs.FerryIcon)
            {
                return ApiMode.Ferry;
            }
            if(iconGlyph == FontIconGlyphs.MetroIcon)
            {
                return ApiMode.Subway;
            }
            if(iconGlyph == FontIconGlyphs.TrainIcon)
            {
                return ApiMode.Rail;
            }
            if(iconGlyph == FontIconGlyphs.TramIcon)
            {
                return ApiMode.Tram;
            }
            if(iconGlyph == FontIconGlyphs.WalkIcon)
            {
                return ApiMode.Walk;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
