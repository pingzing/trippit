using DigiTransit10.ViewModels.ControlViewModels;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class StopSearchContent : UserControl
    {
        private VisualState _currentState;

        public StopSearchContentViewModel ViewModel => DataContext as StopSearchContentViewModel;
        public static readonly DependencyProperty IsSearchBoxVisibleProperty = DependencyProperty.Register(nameof(IsSearchBoxVisible), typeof(bool), typeof(StopSearchContent), new PropertyMetadata(false));
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> SearchBoxTextChanged;
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> SearchBoxQuerySubmitted;

        public StopSearchContent()
        {
            this.InitializeComponent();
            this.Common.CurrentStateChanged += (s, args) => _currentState = args.NewState;
            this.Loaded += StopSearchContent_Loaded;
            this.Unloaded += StopSearchContent_Unloaded;
        }

        private void StopSearchContent_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel?.LoadedCommand.Execute(null);
        }

        private void StopSearchContent_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel?.UnloadedCommand.Execute(null);
        }

        private void StopsSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchBoxTextChanged?.Invoke(sender, args);
        }

        private void StopsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SearchBoxQuerySubmitted?.Invoke(sender, args);
        }

        private void StopsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (StopSearchElementViewModel vm in e.RemovedItems.OfType<StopSearchElementViewModel>())
            {
                vm.IsSelected = false;
            }
            foreach (StopSearchElementViewModel vm in e.AddedItems.OfType<StopSearchElementViewModel>())
            {
                vm.IsSelected = true;
            }

        }

        public bool IsSearchBoxVisible
        {
            get { return (bool)GetValue(IsSearchBoxVisibleProperty); }
            set { SetValue(IsSearchBoxVisibleProperty, value); }
        }
    }
}
