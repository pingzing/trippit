using Windows.UI.Xaml;

namespace DigiTransit10.VisualStateFramework
{
    public class DataTrigger : StateTriggerBase
    {
        private string _viewModelStateName;
        public string ViewModelStateName
        {
            get { return _viewModelStateName; }
            set
            {
                if(_viewModelStateName != value)
                {
                    _viewModelStateName = value;
                }
                if (ViewModel != null && ViewModel.CurrentStateName == null && _viewModelStateName != null)
                {
                    ViewModel.CurrentStateName = ViewModelStateName;
                }
            }
        }

        private bool _isDefaultState;
        public bool IsDefaultState
        {
            get { return _isDefaultState; }
            set
            {
                if(_isDefaultState != value)
                {
                    _isDefaultState = value;
                }
                if(ViewModel != null && ViewModel.CurrentStateName == null && ViewModelStateName != null)
                {
                    ViewModel.CurrentStateName = ViewModelStateName;
                }
            }
        }

        private StateAwareViewModel _viewModel;
        public StateAwareViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if(value == null)
                {
                    return;
                }                
                _viewModel = value;
                _viewModel.VmStateChangeRequested += VmStateChangeRequested;

                if (_viewModel.CurrentStateName == null && ViewModelStateName != null && IsDefaultState)
                {
                    ViewModel.CurrentStateName = ViewModelStateName;
                }
            }
        }

        private void VmStateChangeRequested(StateAwareViewModel viewModel, VmStateChangedEventArgs args)
        {
            if (viewModel.CurrentStateName != args.NewStateName)
            {                
                SetActive(args.NewStateName.Equals(ViewModelStateName));
                viewModel.CurrentStateName = args.NewStateName;
            }
        }
    }
}
