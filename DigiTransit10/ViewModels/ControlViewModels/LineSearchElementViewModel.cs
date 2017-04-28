using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class LineSearchElementViewModel : BindableBase
    {       
        public TransitLine BackingLine { get; set; }

        private ObservableCollection<TransitStop> _visibleStops = new ObservableCollection<TransitStop>();
        public ObservableCollection<TransitStop> VisibleStops
        {
            get { return _visibleStops; }
            set { Set(ref _visibleStops, value); }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                Set(ref _isSelected, value);
                if (_isSelected)
                {
                    VisibleStops.AddRange(BackingLine.Stops);
                }
                else
                {
                    VisibleStops.Clear();
                }
            }
        }        

        public LineSearchElementViewModel()
        {
            //Designer is now happy
        }        
    }
}
