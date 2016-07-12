using Windows.UI.Xaml;

namespace DigiTransit10.VisualStateFramework
{
    public class DataTrigger : StateTriggerBase
    {
        public string ViewModelStateName { get; set; }

        private StateAwareViewModel _viewModel;
        public StateAwareViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                if(value == null)
                {
                    return;
                }
                _viewModel = value;
                _viewModel.VmStateChangeRequested += VmStateChangeRequested;
            }
        }        

        private void VmStateChangeRequested(StateAwareViewModel viewModel, VmStateChangedEventArgs args)
        {
            SetActive(args.NewStateName.Equals(ViewModelStateName));
            viewModel.CurrentStateName = args.NewStateName;
        }
    }
}
