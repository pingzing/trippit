using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using DigiTransit10.Backend;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;
using MetroLog;
using Template10.Services.NavigationService;

namespace DigiTransit10.Services
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

                ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger("GlobalLogger");
                SimpleIoc.Default.Register<ILogger>(() => logger);

                SimpleIoc.Default.Register<INetworkService>(() => new NetworkService(networkClient, settingsService, logger));
                SimpleIoc.Default.Register<IMessenger>(() => Messenger.Default);
                SimpleIoc.Default.Register<IGeolocationService>(() => new GeolocationService());
                SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
                SimpleIoc.Default.Register<IFavoritesService>(() => new FavoritesService(settingsService, fileService));
                SimpleIoc.Default.Register<ICustomFontService>(() => new CustomFontService(fileService));
                SimpleIoc.Default.Register<ITileService>(() => new TileService(settingsService, logger));
            }
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<TripFormViewModel>();
            SimpleIoc.Default.Register<TripResultViewModel>();
            SimpleIoc.Default.Register<FavoritesViewModel>();
        }

        //Viewmodels
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public TripFormViewModel TripForm => ServiceLocator.Current.GetInstance<TripFormViewModel>();
        public TripResultViewModel TripResult => ServiceLocator.Current.GetInstance<TripResultViewModel>();
        public FavoritesViewModel Favorites => ServiceLocator.Current.GetInstance<FavoritesViewModel>();

        public async Task CleanupAsync()
        {
            //Serialize data that needs serializing, etc etc
            SimpleIoc.Default.GetInstance<SettingsServices.SettingsService>().FlushPinnedFavoriteIdsToStorage();
            await SimpleIoc.Default.GetInstance<IFavoritesService>().FlushFavoritesAsync();
        }
    }
}
