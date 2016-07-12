using Template10.Mvvm;

namespace DigiTransit10.VisualStateFramework
{
    public abstract class StateAwareViewModel : ViewModelBase
    {
        /// <summary>
        /// A ViewModel that can set VisualStates by making use of the <see cref="VmStateChangeRequested"/> event.
        /// </summary>
        protected StateAwareViewModel() : base()
        {

        }

        /// <summary>
        /// Callback performed when <see cref="StateAwareViewModel.VmStateChangeRequested"/> is fired.
        /// Implemented by <see cref="DataTrigger"/> by default.
        /// </summary>
        /// <param name="viewModel">ViewModel associated with the View whose VisualState will be changed.</param>
        /// <param name="args">The name of the VisualState to change to.</param>
        public delegate void VmStateChangeHandler(StateAwareViewModel viewModel, VmStateChangedEventArgs args);

        /// <summary>
        /// To be implemented by inheriting ViewModels and invoked to request that the associated View change Visual State.
        /// </summary>
        public abstract event VmStateChangeHandler VmStateChangeRequested;

        /// <summary>
        /// The name of the current VisualState. Automatically updated when changed by a <see cref="DataTrigger"/>.
        /// </summary>
        public string CurrentStateName { get; set; }
    }
}