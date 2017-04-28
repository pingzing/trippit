using DigiTransit10.ViewModels.ControlViewModels;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.Controls
{
    public sealed partial class LineSearchContent : UserControl
    {
        private DispatcherTimer _typingTimer = new DispatcherTimer();

        public LineSearchContentViewModel ViewModel => DataContext as LineSearchContentViewModel;

        public LineSearchContent()
        {
            this.InitializeComponent();
            _typingTimer.Interval = TimeSpan.FromMilliseconds(500);
            _typingTimer.Tick += TypingTimer_Tick;
        }        

        private void LinesSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            _typingTimer.Stop();
            _typingTimer.Start();
        }

        private void LinesSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _typingTimer.Stop();
            ViewModel.GetLinesCommand.Execute(args.QueryText);
        }

        private void TypingTimer_Tick(object sender, object e)
        {
            _typingTimer.Stop();
            ViewModel.GetLinesCommand.Execute(this.LinesSearchBox.Text);
        }

        private void LinesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (LineSearchElementViewModel vm in e.RemovedItems.OfType<LineSearchElementViewModel>())
            {
                vm.IsSelected = false;               
            }

            foreach (LineSearchElementViewModel vm in e.AddedItems.OfType<LineSearchElementViewModel>())
            {
                vm.IsSelected = true;
            }
        }
    }
}
