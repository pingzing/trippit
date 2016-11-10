using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        private VisualState _narrowVisualState;
        
        private DispatcherTimer _linesTypingThrottle = new DispatcherTimer();
        private string _linesSearchText;
        private DispatcherTimer _stopsTypingThrottle = new DispatcherTimer();
        private string _stopsSearchText;

        public SearchViewModel ViewModel => DataContext as SearchViewModel;

        public SearchPage()
        {
            this.InitializeComponent();            

            _linesTypingThrottle.Interval = TimeSpan.FromMilliseconds(500);
            _linesTypingThrottle.Tick += LinesTypingThrottle_Tick;

            _stopsTypingThrottle.Interval = TimeSpan.FromMilliseconds(500);
            _stopsTypingThrottle.Tick += StopsTypingThrottle_Tick;
            _narrowVisualState = AdaptiveVisualStateGroup.States.First(x => x.Name == "VisualStateNarrow");
            this.Unloaded += SearchPage_Unloaded1;        
        }

        private void SearchPage_Unloaded1(object sender, RoutedEventArgs e)
        {
            Bindings.StopTracking();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Register<MessageTypes.CenterMapOnGeoposition>(this, CenterMapOnLocation);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PageMap = null;
            Messenger.Default.Unregister<MessageTypes.CenterMapOnGeoposition>(this);
            base.OnNavigatedFrom(e);
        }

        private async void CenterMapOnLocation(MessageTypes.CenterMapOnGeoposition args)
        {
            const double initialZoomAdjustment = 0.01;
            const double narrowZoomAdjustment = 0.005;

            // Create a box surrounding the specified point to emulate a zoom level on the map.
            // We're using a simulated bounding box, because we need to be able to specify a margin,
            // to accommodate either of the the floating content panels.
            BasicGeoposition northwest = BasicGeopositionExtensions.Create
            (
                0.0,
                args.Position.Longitude - initialZoomAdjustment,
                args.Position.Latitude + initialZoomAdjustment
            );
            BasicGeoposition southeast = BasicGeopositionExtensions.Create
            (
                0.0,
                args.Position.Longitude + initialZoomAdjustment,
                args.Position.Latitude - initialZoomAdjustment
            );            
            if (AdaptiveVisualStateGroup.CurrentState == _narrowVisualState)
            {
                //Zoom in a little further when in the narrow view, otherwise we're in a little too close 
                //for the narrow field of view
                northwest.Longitude += narrowZoomAdjustment;
                northwest.Latitude -= narrowZoomAdjustment;

                southeast.Longitude -= narrowZoomAdjustment;
                southeast.Latitude += narrowZoomAdjustment;

                GeoboundingBox box = new GeoboundingBox(northwest, southeast);
                if (NarrowSearchPanel.IsOpen)
                {
                    double bottomMargin = NarrowSearchPanel.ExpandedHeight;                    
                    await PageMap.TrySetViewBoundsAsync(box, new Thickness(0, 0, 0, bottomMargin), MapAnimationKind.None);
                }
                else
                {                    
                    await PageMap.TrySetViewBoundsAsync(box, new Thickness(0, 0, 0, 0), MapAnimationKind.None);
                }
            }
            else
            {
                GeoboundingBox box = new GeoboundingBox(northwest, southeast);
                await PageMap.TrySetViewBoundsAsync(box, new Thickness(400, 0, 0, 0), MapAnimationKind.None);
            }
        }

        private void NarrowSearchPanel_Loaded(object sender, RoutedEventArgs e)
        {
            NarrowSearchPanel.ExpandedHeight = this.ActualHeight * .50;
            this.SizeChanged += SearchPage_SizeChanged;
            this.Unloaded += SearchPage_Unloaded;
            if (this.Parent != null)
            {
                (this.Parent as FrameworkElement).SizeChanged += Parent_SizeChanged;
            }
        }

        private void SearchPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent != null)
            {
                (this.Parent as FrameworkElement).SizeChanged -= Parent_SizeChanged;
            }
        }

        private void SearchPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NarrowSearchPanel.ExpandedHeight = this.ActualHeight * .50;
            ClipToBounds();
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipToBounds();
        }

        private void ClipToBounds()
        {
            this.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight)
            };
        }        

        private void StopsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _stopsTypingThrottle.Stop();
            if (ViewModel?.SearchStopsCommand.CanExecute(args.QueryText) == true)
            {
                ViewModel.SearchStopsCommand.Execute(args.QueryText);
            }
        }        

        private void StopsSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string searchText = sender.Text;
            if (args.CheckCurrent()
                && ViewModel?.SearchStopsCommand.CanExecute(searchText) == true)
            {
                _stopsSearchText = searchText;
                _stopsTypingThrottle.Stop();
                _stopsTypingThrottle.Start();
            }
        }

        private void StopsTypingThrottle_Tick(object sender, object e)
        {
            _stopsTypingThrottle.Stop();
            ViewModel.SearchStopsCommand.Execute(_stopsSearchText);
        }        

        private void LinesSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _linesTypingThrottle.Stop();
            if(ViewModel?.SearchLinesCommand.CanExecute(args.QueryText) == true)
            {
                ViewModel.SearchLinesCommand.Execute(args.QueryText);
            }
        }

        private void LinesSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string searchText = sender.Text;
            if (args.CheckCurrent()
                && ViewModel?.SearchLinesCommand.CanExecute(searchText) == true)
            {
                _linesSearchText = searchText;
                _linesTypingThrottle.Stop();
                _linesTypingThrottle.Start();
            }
        }

        private void LinesTypingThrottle_Tick(object sender, object e)
        {
            _linesTypingThrottle.Stop();
            ViewModel.SearchLinesCommand.Execute(_linesSearchText);
        }
        
        private void PageMap_MapCenterChanged(MapControl sender, object args)
        {
            //nothing for now. if we never need this, just delete this handler.
        }

        private void PageMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            var flyout = (MenuFlyout)Flyout.GetAttachedFlyout(PageMap);
            ((MenuFlyoutItem)flyout.Items[0]).CommandParameter = args.Location;
            flyout.ShowAt(this, args.Position);
        }

        private void SearchPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = (Pivot)sender;
            SearchSection selectedSection = SearchSection.Nearby;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    selectedSection = SearchSection.Nearby;
                    break;
                case 1:
                    selectedSection = SearchSection.Lines;
                    break;
                case 2:
                    selectedSection = SearchSection.Stops;
                    break;
            }

            ViewModel.SectionChangedCommand.Execute(selectedSection);
        }

        private void LinesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //toggle view state of selected element
            //trigger selection changed in viewmodel
            ApiRoute selectedItem = (ApiRoute)e.AddedItems.FirstOrDefault();
            if(ViewModel?.UpdateSelectedLineCommand.CanExecute(selectedItem) == true)
            {
                ViewModel.UpdateSelectedLineCommand.Execute(selectedItem);
            }
        }
    }
}
