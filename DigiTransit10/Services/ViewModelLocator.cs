using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Services
{
    public class ViewModelLocator
    {
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


            }
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}
