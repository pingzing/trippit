using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DigiTransit10.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        public Shell()
        {
            Instance = this;
            InitializeComponent();

            HamburgerMenu.Loaded += HamburgerMenu_Loaded;
        }

        private void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (HamburgerMenu.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                HamburgerMenu.HamburgerButtonVisibility = Visibility.Collapsed;
                HamburgerMenu.HamburgerBackground = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                HamburgerMenu.HamburgerButtonVisibility = Visibility.Visible;
                HamburgerMenu.HamburgerBackground = (SolidColorBrush)Application.Current.Resources["SystemControlHighlightAccentBrush"];
            }

            HamburgerMenu.DisplayModeChanged += MyHamburgerMenu_OnDisplayModeChanged;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }

        //We don't use the Hamburger menu on the phone, so hide the button in narrow view.
        private void MyHamburgerMenu_OnDisplayModeChanged(object sender, ChangedEventArgs<SplitViewDisplayMode> e)
        {
            if (HamburgerMenu.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                HamburgerMenu.HamburgerButtonVisibility = Visibility.Collapsed;
                HamburgerMenu.HamburgerBackground = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                HamburgerMenu.HamburgerButtonVisibility = Visibility.Visible;
                HamburgerMenu.HamburgerBackground = (SolidColorBrush)Application.Current.Resources["SystemControlHighlightAccentBrush"];
            }
        }
    }
}

