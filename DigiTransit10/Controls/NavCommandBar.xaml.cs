using Windows.UI.Xaml.Controls;

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
