using System;
using DigiTransit10.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using DigiTransit10.Localization.Strings;

namespace DigiTransit10.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel => this.DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;                          
        }

        private void HideShowOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton button = sender as HyperlinkButton;
            if(button == null)
            {
                return;
            }

            ViewModel?.TripFormViewModel?.ToggleTransitPanelCommand.Execute(null);
        }

        private void PlanTripButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if(AdaptiveVisualStateGroup.CurrentState.Name == "VisualStateNarrow")
            {
                ViewModel.TripFormViewModel.PlanTripNarrowViewCommand.Execute(null);
            }
            else
            {
                ViewModel.TripFormViewModel.PlanTripWideViewCommand.Execute(null);
            }
        }
    }
}
