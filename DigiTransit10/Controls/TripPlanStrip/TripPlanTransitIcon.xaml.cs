using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static DigiTransit10.Models.ApiModels.ApiEnums;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls.TripPlanStrip
{
    public sealed partial class TripPlanTransitIcon : UserControl
    {
        public static readonly DependencyProperty TransitModeProperty =
            DependencyProperty.Register("TransitMode", typeof(ApiMode), typeof(TripPlanTransitIcon), new PropertyMetadata(0));
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
