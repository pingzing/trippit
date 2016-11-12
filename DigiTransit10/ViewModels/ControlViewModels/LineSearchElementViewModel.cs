using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class LineSearchElementViewModel : BindableBase
    {
        private bool _stopsVisible = false;

        public TransitLine BackingLine { get; set; }

        private ObservableCollection<TransitStop> _visibleStops = new ObservableCollection<TransitStop>();
        public ObservableCollection<TransitStop> VisibleStops
        {
            get { return _visibleStops; }
            set { Set(ref _visibleStops, value); }
        }

        public RelayCommand ToggleLineStopsVisibilityCommand => new RelayCommand(ToggleLineStopsVisibility);

        public LineSearchElementViewModel()
        {
            //Designer is now happy
        }

        private void ToggleLineStopsVisibility()
        {
            if(_stopsVisible)
            {
                VisibleStops.Clear();
            }
            else
            {
                VisibleStops.AddRange(BackingLine.Stops);
            }

            _stopsVisible = !_stopsVisible;
        }
    }
}
