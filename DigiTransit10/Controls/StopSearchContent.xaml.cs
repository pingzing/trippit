using DigiTransit10.ViewModels.ControlViewModels;
using System;
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
        private DispatcherTimer _typingTimer = new DispatcherTimer();

        public StopSearchContentViewModel ViewModel => DataContext as StopSearchContentViewModel;
        public static readonly DependencyProperty IsSearchBoxVisibleProperty = DependencyProperty.Register(nameof(IsSearchBoxVisible), typeof(bool), typeof(StopSearchContent), new PropertyMetadata(false));        

        public StopSearchContent()
        {
            this.InitializeComponent();
            this.Common.CurrentStateChanged += (s, args) => _currentState = args.NewState;
            this.Loaded += StopSearchContent_Loaded;
            this.Unloaded += StopSearchContent_Unloaded;
            _typingTimer.Interval = TimeSpan.FromMilliseconds(500);
            _typingTimer.Tick += TypingTimer_Tick;
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
            _typingTimer.Stop();
            _typingTimer.Start();
        }

        private void StopsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _typingTimer.Stop();
            ViewModel.SearchStopsCommand.Execute(args.QueryText);
        }

        private void TypingTimer_Tick(object sender, object e)
        {
            _typingTimer.Stop();
            //ViewModel.SearchStopsCommand.Execute(this.StopsSearchBox.Text);
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
