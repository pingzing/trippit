using Trippit.Controls;
using Trippit.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Trippit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlertsPage : AnimatedPage
    {
        public AlertsViewModel ViewModel => DataContext as AlertsViewModel;

        public AlertsPage()
        {
            this.InitializeComponent();
        }
    }
}
