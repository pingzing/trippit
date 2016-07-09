using DigiTransit10.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripResultsList : UserControl, INotifyPropertyChanged
    {
        public TripResultViewModel ViewModel => DataContext as TripResultViewModel;

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if(_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TripResultsList()
        {
            this.InitializeComponent();
        }        

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void TripPlanStrip_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoading = false;
        }
    }
}
