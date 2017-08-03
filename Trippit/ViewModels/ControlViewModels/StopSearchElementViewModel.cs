using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;
using Trippit.Models;

namespace Trippit.ViewModels.ControlViewModels
{
    public class StopSearchElementViewModel : BindableBase
    {
        private IMessenger _messenger;
         
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

        public StopSearchElementViewModel(TransitStop backingStop, IMessenger messenger)
        {
            BackingStop = backingStop;
            _messenger = messenger;
        }        

        private void ViewDetails()
        {
            _messenger.Send(new Helpers.MessageTypes.ViewStopDetails(this));
        }
    }
}
