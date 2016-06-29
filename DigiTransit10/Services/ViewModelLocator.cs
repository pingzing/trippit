using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.SettingsService;
using GalaSoft.MvvmLight.Messaging;

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
                //runtime stuff                
                SimpleIoc.Default.Register<Backend.INetworkClient>(() => new Backend.NetworkClient());
                SimpleIoc.Default.Register<INetworkService>(
                    () => new NetworkService(ServiceLocator.Current.GetInstance<Backend.INetworkClient>())
                );
                
                SimpleIoc.Default.Register(() => SettingsServices.SettingsService.Instance);                
                SimpleIoc.Default.Register<IMessenger>(() => Messenger.Default);

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
    }
}
