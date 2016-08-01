using System.ComponentModel;
using System.Linq;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            this.Loaded += Shell_Loaded;              
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            if (HamburgerMenu.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                HamburgerMenu.HamburgerButtonVisibility = Visibility.Collapsed;
                HamburgerMenu.HamburgerBackground.Opacity = 0;
            }
            else
            {
                HamburgerMenu.HamburgerButtonVisibility = Visibility.Visible;
                HamburgerMenu.HamburgerBackground.Opacity = 1;
            }
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
                HamburgerMenu.HamburgerBackground.Opacity = 0;
            }
            else
            {
                HamburgerMenu.HamburgerButtonVisibility = Visibility.Visible;
                HamburgerMenu.HamburgerBackground.Opacity = 1;
            }
        }
    }
}

