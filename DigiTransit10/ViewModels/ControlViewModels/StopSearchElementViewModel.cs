using DigiTransit10.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class StopSearchElementViewModel : BindableBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        private TransitStop _backingStop;
        public TransitStop BackingStop
        {
            get { return _backingStop; }
            set { Set(ref _backingStop, value); }
        }

        public RelayCommand ViewDetailsCommand => new RelayCommand(ViewDetails);        

        public StopSearchElementViewModel(TransitStop backingStop)
        {
            BackingStop = backingStop;
        }        

        private void ViewDetails()
        {
            //send a message to parent asking it to move into the detailed state.  
        }       
    }
}
