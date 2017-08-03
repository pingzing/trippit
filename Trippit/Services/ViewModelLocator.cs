using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using Trippit.Backend;
using Trippit.ViewModels;

namespace Trippit.Services
{
    public class ViewModelLocator
    {
        private const string LocalSettingsService = "LocalSettingsService";
        private const string RoamingSettingsService = "RoamingSettingsService";

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if(GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                //design-time stuff
            }
            else
            {
                IFileService fileService = new FileService();
                SimpleIoc.Default.Register(() => fileService);

                //runtime stuff                
                var settingsService = SettingsServices.SettingsService.Instance;
                SimpleIoc.Default.Register(() => settingsService);

                INetworkClient networkClient = new NetworkClient();
                SimpleIoc.Default.Register<INetworkClient>(() => networkClient);                

                SimpleIoc.Default.Register<INetworkService>(() => new NetworkService(networkClient, settingsService));
                SimpleIoc.Default.Register<IMessenger>(() => Messenger.Default);
                SimpleIoc.Default.Register<IGeolocationService>(() => new GeolocationService());
                SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
                SimpleIoc.Default.Register<IFavoritesService>(() => new FavoritesService(settingsService, fileService));                
                SimpleIoc.Default.Register<ITileService>(() => new TileService(settingsService));
                SimpleIoc.Default.Register<IAnalyticsService>(() => new AnalyticsService(settingsService));
            }
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<TripFormViewModel>();
            SimpleIoc.Default.Register<TripResultViewModel>();
            SimpleIoc.Default.Register<FavoritesViewModel>();
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<AlertsViewModel>();
        }

        //Viewmodels
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public TripFormViewModel TripForm => ServiceLocator.Current.GetInstance<TripFormViewModel>();
        public TripResultViewModel TripResult => ServiceLocator.Current.GetInstance<TripResultViewModel>();
        public FavoritesViewModel Favorites => ServiceLocator.Current.GetInstance<FavoritesViewModel>();
        public SearchViewModel Search => ServiceLocator.Current.GetInstance<SearchViewModel>();
        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();
        public AlertsViewModel Alerts => ServiceLocator.Current.GetInstance<AlertsViewModel>();

        public async Task CleanupAsync()
        {
            //Serialize data that needs serializing, etc etc
            SimpleIoc.Default.GetInstance<SettingsServices.SettingsService>().FlushPinnedFavoriteIdsToStorage();
            await SimpleIoc.Default.GetInstance<IFavoritesService>().FlushFavoritesAsync();
        }
    }
}
