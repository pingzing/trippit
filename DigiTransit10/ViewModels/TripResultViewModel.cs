using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigiTransit10.Models;
using DigiTransit10.Services;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;

        public ObservableCollection<TripResult> _tripResults = new ObservableCollection<TripResult>();
        public ObservableCollection<TripResult> TripResults
        {
            get { return _tripResults; }
            set { Set(ref _tripResults, value); }
        }  

        public TripResultViewModel(INetworkService networkService)
        {
            _networkService = networkService;
        }
    }
}
