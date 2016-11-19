using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class StopSearchContentViewModel : BindableBase
    {
        public enum StopSearchState { Overview, Details };

        private StopSearchState _currentState;
        public StopSearchState CurrentState
        {
            get { return _currentState; }
            set { Set(ref _currentState, value); }
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
            if (CurrentState == StopSearchState.Details)
            {
                _currentState = StopSearchState.Overview;
                e.Handled = true;
                //go to Overview somehow                
            }
        }
    }
}
