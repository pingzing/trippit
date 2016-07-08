using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static DigiTransit10.Models.ApiModels.ApiEnums;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripPlanTransitMethod : UserControl
    {
        public static readonly DependencyProperty TransitModeProperty =
            DependencyProperty.Register("TransitMode", typeof(ApiMode), typeof(TripPlanTransitMethod), new PropertyMetadata(0));
        public ApiMode TransitMode
        {
            get { return (ApiMode)GetValue(TransitModeProperty); }
            set { SetValue(TransitModeProperty, value); }
        }                
        
        public static readonly DependencyProperty ShortNameProperty =
            DependencyProperty.Register("ShortName", typeof(string), typeof(TripPlanTransitMethod), new PropertyMetadata(null, new PropertyChangedCallback(ShortNameChanged)));

        private static void ShortNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripPlanTransitMethod thisControl = d as TripPlanTransitMethod;
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
            DependencyProperty.Register("Distance", typeof(float?), typeof(TripPlanTransitMethod), new PropertyMetadata(null, new PropertyChangedCallback(DistanceChanged)));

        private static void DistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripPlanTransitMethod thisControl = d as TripPlanTransitMethod;
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

        public TripPlanTransitMethod()
        {
            this.InitializeComponent();
        }
    }
}
