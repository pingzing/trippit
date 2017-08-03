using Trippit.ViewModels.ControlViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Controls
{
    public sealed partial class StopSearchElement : UserControl
    {
        public StopSearchElementViewModel ViewModel => this.DataContext as StopSearchElementViewModel;

        public StopSearchElement()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();       
        }
    }
}
