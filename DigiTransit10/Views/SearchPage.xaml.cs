using DigiTransit10.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
        public SearchViewModel ViewModel => DataContext as SearchViewModel;

        public SearchPage()
        {
            this.InitializeComponent();                              
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
                ViewModel.SearchStopsCommand.Execute(searchText);
            }
        }

        private void LinesSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (ViewModel?.SearchLinesCommand.CanExecute(args.QueryText) == true)
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
                ViewModel.SearchLinesCommand.Execute(searchText);
            }
        }

        //todo: add throttle here
        private void PageMap_MapCenterChanged(MapControl sender, object args)
        {                        
            if(selectedPivotIndex != 0)
            {
                return;
            }
            GeoboundingBox boundingBox = PageMap.GetMapViewBoundingBox();
            if(boundingBox == null)
            {
                return;
            }
            if(ViewModel?.UpdateNearbyPlacesCommand.CanExecute(boundingBox) == true)
            {
                ViewModel.UpdateNearbyPlacesCommand.Execute(boundingBox);
            }
        }

        int selectedPivotIndex = 0;
        private void SearchPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = (Pivot)sender;
            selectedPivotIndex = pivot.SelectedIndex;
        }
    }
}
