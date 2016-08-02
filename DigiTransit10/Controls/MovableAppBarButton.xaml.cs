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
    public sealed partial class MovableAppBarButton : AppBarButton, ISortableAppBarButton
    {
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(int), typeof(MovableAppBarButton), new PropertyMetadata(0));
        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty IsSecondaryCommandProperty =
            DependencyProperty.Register("IsSecondaryCommand", typeof(bool), typeof(NavAppBarButton), new PropertyMetadata(false,
                new PropertyChangedCallback(IsSecondaryCommandChanged)));
        private static void IsSecondaryCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavAppBarButton _this = d as NavAppBarButton;
            if (_this == null)
            {
                return;
            }

            bool newIsSecondary = (bool)e.NewValue;
            if (newIsSecondary)
            {
                _this.Width = double.NaN;
                _this.HorizontalAlignment = HorizontalAlignment.Stretch;
            }
            else
            {
                _this.Width = 68;
                _this.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
        public bool IsSecondaryCommand
        {
            get { return (bool)GetValue(IsSecondaryCommandProperty); }
            set { SetValue(IsSecondaryCommandProperty, value); }
        }

        public MovableAppBarButton()
        {
            this.InitializeComponent();
        }

        public int CompareTo(ISortableAppBarButton other)
        {
            return this.Position.CompareTo(other.Position);
        }
    }
}
