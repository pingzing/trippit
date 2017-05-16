using DigiTransit10.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;

namespace DigiTransit10.ViewModels
{
    public class AlertsViewModel : ViewModelBase
    {
        private ObservableCollection<TransitTrafficAlert> _trafficAlerts = new ObservableCollection<TransitTrafficAlert>();
        public ObservableCollection<TransitTrafficAlert> TrafficAlerts
        {
            get { return _trafficAlerts; }
            set { Set(ref _trafficAlerts, value); }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            TrafficAlerts.Clear();

            List<TransitTrafficAlert> alerts = parameter as List<TransitTrafficAlert>;
            if (alerts != null)
            {
                TrafficAlerts = new ObservableCollection<TransitTrafficAlert>(alerts);
            }


            return base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
