using DigiTransit10.Helpers;
using DigiTransit10.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using static DigiTransit10.Helpers.MessageTypes;

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

        private RelayCommand<TransitTrafficAlert> _lineClickedCommand;
        public RelayCommand<TransitTrafficAlert> LineClickedCommand => _lineClickedCommand ?? (_lineClickedCommand = new RelayCommand<TransitTrafficAlert>(LineClicked));

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

        private void LineClicked(TransitTrafficAlert clicked)
        {
            LineSearchRequested message = new LineSearchRequested(clicked, typeof(AlertsViewModel));
            NavigationService.NavigateAsync(typeof(Views.SearchPage), message);
        }
    }
}
