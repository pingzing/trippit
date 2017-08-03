using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using Trippit.Helpers;
using Trippit.Models;
using Trippit.ViewModels.ControlViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Trippit.ExtensionMethods.MapElementExtensions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Controls
{
    public sealed partial class StopSearchContent : UserControl
    {
        private VisualState _currentState;
        private DispatcherTimer _typingTimer = new DispatcherTimer();

        public StopSearchContentViewModel ViewModel => DataContext as StopSearchContentViewModel;

        public static readonly DependencyProperty IsSearchBoxVisibleProperty = DependencyProperty.Register(nameof(IsSearchBoxVisible), typeof(bool), typeof(StopSearchContent), new PropertyMetadata(false));
        public bool IsSearchBoxVisible
        {
            get { return (bool)GetValue(IsSearchBoxVisibleProperty); }
            set { SetValue(IsSearchBoxVisibleProperty, value); }
        }

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
            ViewModel.SearchStopsCommand.Execute(this.StopsSearchBox.Text);
        }

        private void StopsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (StopSearchElementViewModel vm in e.RemovedItems.OfType<StopSearchElementViewModel>())
            {
                if (vm.IsSelected)
                {
                    Messenger.Default.Send(new MessageTypes.SetIconState(vm.BackingStop.Id, MapIconState.None));
                    vm.IsSelected = false;
                }
            }

            ListView listView = sender as ListView;            
            foreach (StopSearchElementViewModel vm in e.AddedItems.OfType<StopSearchElementViewModel>())
            {                
                if (!vm.IsSelected)
                {
                    Messenger.Default.Send(new MessageTypes.SetIconState(vm.BackingStop.Id, MapIconState.Selected));
                    ListView list = (ListView)sender;
                    list.ScrollIntoView(vm, ScrollIntoViewAlignment.Default);
                    vm.IsSelected = true;
                }
                listView.ScrollIntoView(vm, ScrollIntoViewAlignment.Leading);
            }

        }

        private void DetailsLinesGrid_Click(object sender, ItemClickEventArgs e)
        {
            var line = e.ClickedItem as ITransitLine;
            if (line != null)
            {
                ViewModel.ViewLineCommand.Execute(line);
            }
        }
    }
}
