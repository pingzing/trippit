using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Controls
{
    public sealed partial class MovableAppBarToggleButton : AppBarToggleButton, ISortableAppBarButton
    {
        public static readonly DependencyProperty PositionProperty =
           DependencyProperty.Register("Position", typeof(int), typeof(MovableAppBarToggleButton), new PropertyMetadata(0));
        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty IsSecondaryCommandProperty =
            DependencyProperty.Register("IsSecondaryCommand", typeof(bool), typeof(MovableAppBarToggleButton), new PropertyMetadata(false,
                new PropertyChangedCallback(IsSecondaryCommandChanged)));
        private static void IsSecondaryCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MovableAppBarToggleButton _this = d as MovableAppBarToggleButton;
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

                /* HACK: Force a binding update in case the button should NOT be enabled.
                 * Reason: If a button is a SecondaryCommand, its IsEnabled is forcibly set 
                 * to false whenever the overflow panel is hidden. If we just move the button 
                 * from SecondaryCommands to PrimaryCommands, whatever voodoo magic the 
                 * CommandBar uses to set IsEnabled correctly never gets cast. So set it to IsEnabled,
                 * refresh the DataContext, and hope that nobody has manually set IsEnabled=false.*/
                _this.IsEnabled = true;
                var savedDataContext = _this.DataContext;
                _this.DataContext = null;
                _this.DataContext = savedDataContext;
            }
        }
        public bool IsSecondaryCommand
        {
            get { return (bool)GetValue(IsSecondaryCommandProperty); }
            set { SetValue(IsSecondaryCommandProperty, value); }
        }

        public MovableAppBarToggleButton()
        {
            this.InitializeComponent();
        }

        public int CompareTo(ISortableAppBarButton other)
        {
            return this.Position.CompareTo(other.Position);
        }
    }
}
