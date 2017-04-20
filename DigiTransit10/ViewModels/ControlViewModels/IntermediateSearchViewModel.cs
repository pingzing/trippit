using DigiTransit10.Models;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.
    ControlViewModels
{
    public class IntermediateSearchViewModel : BindableBase
    {
        private readonly TripFormViewModel _parentVm;

        public RelayCommand SwapLocationsCommand => new RelayCommand(SwapLocations);        
        public RelayCommand RemoveIntermediateCommand => new RelayCommand(RemoveIntermediate);

        private IPlace _intermediatePlace;
        public IPlace IntermediatePlace
        {
            get { return _intermediatePlace; }
            set
            {
                Set(ref _intermediatePlace, value);
                _parentVm.PlanTripCommand.RaiseCanExecuteChanged();
            }
        }

        public IntermediateSearchViewModel()
        {
            //Only for designer friendliness
        }

        public IntermediateSearchViewModel(TripFormViewModel parentVm)
        {
            _parentVm = parentVm;
        }

        public IntermediateSearchViewModel(TripFormViewModel parentVm, IPlace intermediatePlace)
        {
            _parentVm = parentVm;
            IntermediatePlace = intermediatePlace;
        }

        private void SwapLocations()
        {
            _parentVm.SwapIntermediateLocationCommand.Execute(this);
        }        

        private void RemoveIntermediate()
        {
            _parentVm.RemoveIntermediateCommand.Execute(this);
        }
    }
}
