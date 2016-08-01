using Windows.UI.Xaml;
using System.Threading.Tasks;
using DigiTransit10.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using Windows.UI.Xaml.Data;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Threading;
using Windows.ApplicationModel;
using DigiTransit10.Helpers;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;

namespace DigiTransit10
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        /// <summary>
        /// App-wide easy access to the ViewModelLocator.
        /// </summary>
        public ViewModelLocator Locator => Current.Resources["Locator"] as ViewModelLocator;

        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);            

            #region App settings

            var _settings = SettingsService.Instance;            
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;
            RequestedTheme = SettingsService.Instance.AppTheme;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {                
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);                

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };                
            }

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(300, 800));

            this.SessionState = new StateItems(); //apparently this needs to be initialized by hand            

            DispatcherHelper.Initialize();
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here

            // end here

            NavigationService.Navigate(typeof(Views.MainPage));
            await Task.CompletedTask;
        }

        public override async Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            Locator.Cleanup();
            
            await base.OnSuspendingAsync(s, e, prelaunchActivated);
        }
    }
}

