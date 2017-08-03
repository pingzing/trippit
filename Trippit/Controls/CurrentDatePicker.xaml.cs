using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Controls
{
    public sealed partial class CurrentDatePicker : UserControl
    {
        private readonly string UseCurrentDateStateKey;
        private readonly string UseCustomDateStateKey;
        
        public static readonly DependencyProperty UseCurrentDateProperty =
            DependencyProperty.Register(nameof(UseCurrentDate), typeof(bool), typeof(CurrentDatePicker), new PropertyMetadata(true,
                new PropertyChangedCallback(OnCurrentDateChanged)));

        private static void OnCurrentDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrentDatePicker _this = d as CurrentDatePicker;
            if (_this == null)
            {
                return;
            }

            if (!(e.NewValue is bool))
            {
                return;
            }

            bool newUseCurrentDate = (bool)e.NewValue;

            if (newUseCurrentDate)
            {
                _this.UnderPicker.IsEnabled = false;
                _this.WidenAnimation.KeyFrames[0].Value = _this.ControlRoot.ActualWidth;
                if (_this.ControlRoot.ActualWidth > 0)
                {
                    _this.ScaleUpBoxFrame.Value = _this.NarrowColumn.Width.Value / _this.ControlRoot.ActualWidth;
                }
                if (_this.Common.CurrentState.Name == _this.UseCurrentDateStateKey)
                {
                    VisualStateManager.GoToState(_this, _this.UseCurrentDateStateKey, false);
                }
                else
                {
                    VisualStateManager.GoToState(_this, _this.UseCurrentDateStateKey, true);
                }                
            }
            else
            {
                _this.Date = DateTime.Today;
                _this.UnderPicker.IsEnabled = true;
                if (_this.ControlRoot.ActualWidth > 0)
                {
                    _this.ScaleDownBoxFrame.Value = _this.NarrowColumn.Width.Value / _this.ControlRoot.ActualWidth;
                }
                if (_this.Common.CurrentState.Name == _this.UseCustomDateStateKey)
                {
                    VisualStateManager.GoToState(_this, _this.UseCustomDateStateKey, false);
                }
                else
                {
                    VisualStateManager.GoToState(_this, _this.UseCustomDateStateKey, true);
                }
            }
        }
        public bool UseCurrentDate
        {
            get { return (bool)GetValue(UseCurrentDateProperty); }
            set { SetValue(UseCurrentDateProperty, value); }
        }
                
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register(nameof(Date), typeof(DateTime), typeof(CurrentDatePicker), new PropertyMetadata(DateTime.Today, 
                new PropertyChangedCallback(OnDateChanged)));

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrentDatePicker _this = d as CurrentDatePicker;
            if (_this == null)
            {
                return;
            }

            if (!(e.NewValue is DateTimeOffset))
            {
                return;
            }

            DateTimeOffset newDate = (DateTimeOffset)e.NewValue;
            _this.UnderPicker.Date = newDate;
        }

        public DateTimeOffset Date
        {
            get { return (DateTimeOffset)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public CurrentDatePicker()
        {
            this.InitializeComponent();
            UseCurrentDateStateKey = this.Common.States[0].Name;
            UseCustomDateStateKey = this.Common.States[1].Name;
            VisualStateManager.GoToState(this, UseCurrentDateStateKey, false);
        }

        private void UnderPicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            if (e.NewDate == this.Date)
            {
                return;
            }
            this.Date = e.NewDate;
        }

        private void CurrentDateButton_Click(object sender, RoutedEventArgs e)
        {
            this.UseCurrentDate = !this.UseCurrentDate; // triggers state transition
        }
    }
}
