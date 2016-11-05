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
using Windows.Foundation.Metadata;
using DigiTransit10.ExtensionMethods;
using Microsoft.HockeyApp;
using MetroLog;
using MetroLog.Targets;
using System.Diagnostics;
using System;
using DigiTransit10.Models;
using Newtonsoft.Json;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;
using DigiTransit10.Views;

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

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());

            HockeyClient.Current.Configure("c2a732e8165446bc81e0ea6087509c2b");
            //todo: figure out if there's a way to make this work
                //.SetExceptionDescriptionLoader((Exception ex) =>
                //{
                //    return $"Current log: {LogManagerFactory.DefaultLogManager.GetCompressedLogs().Result.}";
                //});
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {            
            if (Window.Current.Content as ModalDialog == null)
            {
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    StatusBar.GetForCurrentView().HideAsync().DoNotAwait();
                }

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
            
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(250, 600));

            this.SessionState = new StateItems(); //apparently this needs to be initialized by hand            

            DispatcherHelper.Initialize();
            await Task.CompletedTask;
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
    }
}

