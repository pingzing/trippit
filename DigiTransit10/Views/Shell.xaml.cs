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

            HamburgerMenu.HamburgerButtonVisibility = 
                HamburgerMenu.DisplayMode == SplitViewDisplayMode.Overlay 
                ? Visibility.Collapsed 
                : Visibility.Visible;
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
            HamburgerMenu.HamburgerButtonVisibility = e.NewValue == SplitViewDisplayMode.Overlay 
                ? Visibility.Collapsed 
                : Visibility.Visible;
        }
    }
}

