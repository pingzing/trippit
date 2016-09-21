using DigiTransit10.ExtensionMethods;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class NavAppBarButton : AppBarButton, ISortableAppBarButton
    {
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(int), typeof(NavAppBarButton), new PropertyMetadata(0));
        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavAppBarButton), new PropertyMetadata(false));
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty PageTypeProperty =
            DependencyProperty.Register("PageType", typeof(Type), typeof(NavAppBarButton), new PropertyMetadata(null));
        public Type PageType
        {
            get { return (Type)GetValue(PageTypeProperty); }
            set { SetValue(PageTypeProperty, value); }
        }


        public static readonly DependencyProperty IsSecondaryCommandProperty =
            DependencyProperty.Register("IsSecondaryCommand", typeof(bool), typeof(NavAppBarButton), new PropertyMetadata(false,
                IsSecondaryCommandChanged));
        private static void IsSecondaryCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavAppBarButton _this = d as NavAppBarButton;
            if(_this == null)
            {
                return;
            }

            bool newIsSecondary = (bool)e.NewValue;
            if(newIsSecondary)
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

        public NavAppBarButton()
        {
            this.InitializeComponent();
            this.RegisterPropertyChangedCallback(IsCompactProperty, IsCompactChanged);
        }

        // This is necessary because sometimes the AppBarButton control forgets to do its FREAKIN' JOB
        // and actually update the text labels when the IsCompact property changes. So we have to go in
        // and holds its hand for it...
        TextBlock _labelTextBlock = null;
        private void IsCompactChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (!IsCompact)
            {
                if(_labelTextBlock == null)
                {
                    _labelTextBlock = this.FindChild<TextBlock>("TextLabel");
                }
                if (_labelTextBlock?.Visibility == Visibility.Collapsed)
                {
                    _labelTextBlock.Visibility = Visibility.Visible;
                }
            }
            if(IsCompact)
            {
                if (_labelTextBlock == null)
                {
                    _labelTextBlock = this.FindChild<TextBlock>("TextLabel");
                }
                if(_labelTextBlock?.Visibility == Visibility.Visible)
                {
                    _labelTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        public int CompareTo(ISortableAppBarButton other)
        {
            return this.Position.CompareTo(other.Position);
        }
    }
}
