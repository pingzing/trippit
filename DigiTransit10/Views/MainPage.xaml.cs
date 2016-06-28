using System;
using DigiTransit10.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace DigiTransit10.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel => this.DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;    
            MainPage_SizeChanged(null, null);                    
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.SizeChanged += MainPage_SizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.SizeChanged -= MainPage_SizeChanged;
        }


        //Limit the Result Hub section's max size, otherwise as it grows, it forces the entire view to grow.
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TripResultHubSection != null && TripFormHubSection != null)
            {
                TripResultHubSection.MaxWidth = this.ActualWidth - TripFormHubSection.ActualWidth;
            }
        }
    }
}
