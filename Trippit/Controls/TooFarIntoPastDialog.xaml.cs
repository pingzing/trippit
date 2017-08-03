using Microsoft.Practices.ServiceLocation;
using Trippit.Services.SettingsServices;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Trippit.Controls
{
    public sealed partial class TooFarIntoPastDialog : ContentDialog
    {
        SettingsService _settings = (SettingsService)ServiceLocator.Current.GetService(typeof(SettingsService));

        public TooFarIntoPastDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DontShowAgainCheckbox.IsChecked == true)
            {
                _settings.IsTooFarIntoPastDialogSuppressed = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DontShowAgainCheckbox.IsChecked == true)
            {
                _settings.IsTooFarIntoPastDialogSuppressed = true;
            }
        }
    }
}
