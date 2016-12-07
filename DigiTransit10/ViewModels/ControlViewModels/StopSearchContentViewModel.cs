using DigiTransit10.Models;
using DigiTransit10.VisualStateFramework;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class StopSearchContentViewModel : StateAwareViewModel
    {
        public enum StopSearchState { Overview, Details };
        private StopSearchState _currentState;

        private ObservableCollection<StopSearchElementViewModel> _stopsResultList = new ObservableCollection<StopSearchElementViewModel>();
        public ObservableCollection<StopSearchElementViewModel> StopsResultList
        {
            get { return _stopsResultList; }
            set { Set(ref _stopsResultList, value); }
        }

        public RelayCommand LoadedCommand => new RelayCommand(Loaded);
        public RelayCommand UnloadedCommand => new RelayCommand(Unloaded);        

        private void Loaded()
        {
            BootStrapper.BackRequested += BootStrapper_BackRequested;
        }

        private void Unloaded()
        {
            BootStrapper.BackRequested -= BootStrapper_BackRequested;
        }

        private void BootStrapper_BackRequested(object sender, HandledEventArgs e)
        {
            if (_currentState == StopSearchState.Details)
            {
                _currentState = StopSearchState.Overview;
                e.Handled = true;
                RaiseStateChanged(StopSearchState.Overview);
            }
        }

        public override event VmStateChangeHandler VmStateChangeRequested;
        private void RaiseStateChanged(StopSearchState newState)
        {
            VmStateChangeRequested?.Invoke(this, new VmStateChangedEventArgs(newState.ToString()));
        }
    }
}
