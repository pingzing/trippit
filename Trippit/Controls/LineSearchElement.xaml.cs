using Trippit.ViewModels.ControlViewModels;
using Windows.UI.Xaml.Controls;

namespace Trippit.Controls
{
    public sealed partial class LineSearchElement : UserControl
    {
        public LineSearchElementViewModel ViewModel => this.DataContext as LineSearchElementViewModel;

        public LineSearchElement()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }
    }
}
