using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class NavCommandBar : CommandBar
    {
        public NavCommandBar()
        {
            this.InitializeComponent();
            Views.Busy.BusyChanged += BusyView_BusyChanged;
        }

        private void BusyView_BusyChanged(object sender, bool newIsBusy)
        {
            this.IsEnabled = !newIsBusy;
        }
    }
}
