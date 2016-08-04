using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using System.Reflection;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class CurrentTimePicker : UserControl
    {
        public static readonly DependencyProperty UseCurrentTimeProperty =
            DependencyProperty.Register("UseCurrentTime", typeof(bool), typeof(CurrentTimePicker), new PropertyMetadata(true,
                new PropertyChangedCallback(OnUseCurrentTimeChanged)));
        private static void OnUseCurrentTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrentTimePicker _this = d as CurrentTimePicker;
            if (_this == null)
            {
                return;
            }

            if(!(e.NewValue is bool))
            {
                return;
            }
            bool newUseCurrentTime = (bool)e.NewValue;

            if(newUseCurrentTime)
            {
                _this.UnderPicker.IsEnabled = false;
                _this.CustomToCurrentWidthAnimatioin.KeyFrames[1].Value = _this.ControlRoot.ActualWidth;
                if (_this.Common.CurrentState.Name == "UseCurrentTimeState")
                {
                    VisualStateManager.GoToState(_this, "UseCurrentTimeState", false);
                }
                else
                {
                    VisualStateManager.GoToState(_this, "UseCurrentTimeState", true);
                }
            }
            else
            {
                _this.Time = DateTime.Now.TimeOfDay;
                _this.UnderPicker.IsEnabled = true;
                _this.CurrentToCustomWidthAnimation.KeyFrames[0].Value = _this.CurrentTimeButton.ActualWidth;
                if (_this.Common.CurrentState.Name == "UseCustomTimeState")
                {
                    VisualStateManager.GoToState(_this, "UseCustomTimeState", false);
                }
                else
                {
                    VisualStateManager.GoToState(_this, "UseCustomTimeState", true);
                }
            }
        }
        public bool UseCurrentTime
        {
            get { return (bool)GetValue(UseCurrentTimeProperty); }
            set { SetValue(UseCurrentTimeProperty, value); }
        }                

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(TimeSpan), typeof(CurrentTimePicker), new PropertyMetadata(DateTime.Now.TimeOfDay,
                new PropertyChangedCallback(OnTimeChanged)));        

        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrentTimePicker _this = d as CurrentTimePicker;
            if(_this == null)
            {
                return;
            }

            if(!(e.NewValue is TimeSpan))
            {
                return;
            }
            TimeSpan newTime = (TimeSpan)e.NewValue;
            
            _this.UnderPicker.Time = newTime;           
        }
        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }    

        public CurrentTimePicker()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "UseCurrentTimeState", false);            
        }


        private void UnderPicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            if(e.NewTime == this.Time)
            {
                return;
            }
            this.Time = e.NewTime;
        }

        private void CurrentTimeButton_Click(object sender, RoutedEventArgs e)
        {                        
            this.UseCurrentTime = !this.UseCurrentTime; //triggers state transition animations
        }
    }
}
