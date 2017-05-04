using DigiTransit10.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel => this.DataContext as SettingsViewModel;

        public SettingsPage()
        {
            this.InitializeComponent();            
            this.Loaded += (s, e) =>
            {
                // TODO: Check to make sure we only do this if SelectedItem for
                // these are both null
                ViewModel.SelectedWalkingAmount = ViewModel.WalkingAmounts[2];
                ViewModel.SelectedWalkingSpeed = ViewModel.WalkingSpeeds[2];
            };
        }
    }
}
