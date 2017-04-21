using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class CurrentTimePicker : UserControl
    {
        private readonly string UseCurrentTimeStateKey;
        private readonly string UseCustomTimeStateKey;

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
                _this.WidenAnimation.KeyFrames[0].Value = _this.ControlRoot.ActualWidth;
                if (_this.ControlRoot.ActualWidth > 0)
                {
                    _this.ScaleUpBoxFrame.Value = _this.NarrowColumn.Width.Value / _this.ControlRoot.ActualWidth;
                }
                if (_this.Common.CurrentState.Name == _this.UseCurrentTimeStateKey)
                {
                    VisualStateManager.GoToState(_this, _this.UseCurrentTimeStateKey, false);
                }
                else
                {
                    VisualStateManager.GoToState(_this, _this.UseCurrentTimeStateKey, true);
                }
            }
            else
            {
                _this.Time = DateTime.Now.TimeOfDay;
                _this.UnderPicker.IsEnabled = true;
                if (_this.ControlRoot.ActualWidth > 0)
                {
                    _this.ScaleDownBoxFrame.Value = _this.NarrowColumn.Width.Value / _this.ControlRoot.ActualWidth;
                }
                if (_this.Common.CurrentState.Name == _this.UseCustomTimeStateKey)
                {
                    VisualStateManager.GoToState(_this, _this.UseCustomTimeStateKey, false);
                }
                else
                {
                    VisualStateManager.GoToState(_this, _this.UseCustomTimeStateKey, true);
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
            UseCurrentTimeStateKey = this.Common.States[0].Name;
            UseCustomTimeStateKey = this.Common.States[1].Name;
            VisualStateManager.GoToState(this, UseCurrentTimeStateKey, false);               
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
