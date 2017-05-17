using DigiTransit10.Controls;
using DigiTransit10.ViewModels;
using Windows.UI.Xaml;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : AnimatedPage
    {
        public SettingsViewModel ViewModel => this.DataContext as SettingsViewModel;

        public SettingsPage()
        {
            this.InitializeComponent();            
            this.Loaded += (s, e) =>
            {
                // TODO: Check to make sure we only do this if SelectedItem for
                // these are both null
                if (ViewModel.SelectedWalkingAmount == null)
                {
                    ViewModel.SelectedWalkingAmount = ViewModel.WalkingAmounts[2];
                }

                if (ViewModel.SelectedWalkingSpeed == null)
                {
                    ViewModel.SelectedWalkingSpeed = ViewModel.WalkingSpeeds[2];
                }
            };
            ToBox.Loaded += (s, e) =>
            {
                this.Focus(FocusState.Programmatic);
            };
            ThemeComboBox.Loaded += (s, e) =>
            {
                Bindings.Update();
            };
        }
    }
}
