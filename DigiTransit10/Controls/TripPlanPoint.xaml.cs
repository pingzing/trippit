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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripPlanPoint : UserControl
    {        
        public static readonly DependencyProperty PlaceNameProperty =
            DependencyProperty.Register("PlaceName", typeof(string), typeof(TripPlanPoint), new PropertyMetadata(""));
        public string PlaceName
        {
            get { return (string)GetValue(PlaceNameProperty); }
            set { SetValue(PlaceNameProperty, value); }
        }

        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(long), typeof(TripPlanPoint), new PropertyMetadata(0L));
        public long StartTime
        {
            get { return (long)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }                

        public TripPlanPoint()
        {
            this.InitializeComponent();
        }
    }
}
