using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Services.NavigationService;
using Template10.Utils;
using Trippit.ExtensionMethods;
using Trippit.Helpers;
using Trippit.Models;
using Trippit.Services;
using Trippit.Services.SettingsServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Trippit
{
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

            SettingsService _settings = SettingsService.Instance;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;
            if (SettingsService.Instance.AppTheme != ElementTheme.Default)
            {
                RequestedTheme = SettingsService.Instance.AppTheme.ToApplicationTheme();
            }
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (!(Window.Current.Content is ModalDialog))
            {
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    StatusBar.GetForCurrentView().HideAsync().DoNotAwait();
                }

                // create a new frame 
                INavigationService nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };

                nav.FrameFacade.Navigated += FrameFacade_Navigated;
            }

            // Set up the API key for use by the rest of the app later.
            await DigiTransitApiKey.InitKey();
            
#if DEBUG
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(250, 600));
#else
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(400, 600));
#endif            

            DispatcherHelper.Initialize();
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here

            // end here

            switch(DetermineStartCause(args))
            {
                case AdditionalKinds.SecondaryTile:
                    var tileArgs = args as LaunchActivatedEventArgs;
                    if(tileArgs != null && !String.IsNullOrWhiteSpace(tileArgs.Arguments))
                    {
                        var payload = JsonConvert.DeserializeObject<SecondaryTilePayload>(tileArgs.Arguments);
                        SessionState[NavParamKeys.SecondaryTilePayload] = payload;
                        if (NavigationService.CurrentPageType == typeof(Views.MainPage))
                        {
                            SimpleIoc.Default.GetInstance<IMessenger>().Send(payload);
                        }
                        else
                        {
                            await NavigationService.NavigateAsync(typeof(Views.MainPage));
                        }
                    }
                    else
                    {
                        await NavigationService.NavigateAsync(typeof(Views.MainPage));
                    }

                    break;

                case AdditionalKinds.Toast: //we don't have toasts yet! so just do whatever.                    
                default:
                    await NavigationService.NavigateAsync(typeof(Views.MainPage));
                    break;
            }

            await Task.CompletedTask;
        }

        public override async Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            await Locator.CleanupAsync();

            await base.OnSuspendingAsync(s, e, prelaunchActivated);
        }

        private void FrameFacade_Navigated(object sender, NavigatedEventArgs e)
        {
            // Used to track pageviews here. Now, nada.
        }
    }
}

