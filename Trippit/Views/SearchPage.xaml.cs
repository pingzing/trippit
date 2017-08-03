using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trippit.Controls;
using Trippit.ExtensionMethods;
using Trippit.Helpers;
using Trippit.ViewModels;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Trippit.Views
{
    public sealed partial class SearchPage : AnimatedPage
    {
        private const double FloatingPanelHeightFraction = 0.5;

        private VisualState _narrowVisualState;        

        public SearchViewModel ViewModel => DataContext as SearchViewModel;

        public SearchPage()
        {
            this.InitializeComponent();
            
            _narrowVisualState = AdaptiveVisualStateGroup.States.First(x => x.Name == Constants.VisualStateNarrow);
            this.Unloaded += SearchPage_Unloaded;
        }

        private void SearchPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= SearchPage_Unloaded;
            Bindings.StopTracking();
            this.SizeChanged -= SearchPage_SizeChanged;
            if (this.Parent != null)
            {
                (this.Parent as FrameworkElement).SizeChanged -= Parent_SizeChanged;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Register<MessageTypes.SearchLineSelectionChanged>(this, SearchLineSelectionChanged);
            Messenger.Default.Register<MessageTypes.NearbyListSelectionChanged>(this, NearbyListSelectionChanged);
            Messenger.Default.Register<MessageTypes.StopsListSelectionChanged>(this, StopsListSelectionChanged);
            Messenger.Default.Register<MessageTypes.CenterMapOnGeoposition>(this, CenterMapOnLocation);
            Messenger.Default.Register<MessageTypes.SetIconState>(this, SetMapIconState);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<MessageTypes.SearchLineSelectionChanged>(this, SearchLineSelectionChanged);
            Messenger.Default.Unregister<MessageTypes.NearbyListSelectionChanged>(this, NearbyListSelectionChanged);
            Messenger.Default.Unregister<MessageTypes.StopsListSelectionChanged>(this, StopsListSelectionChanged);
            Messenger.Default.Unregister<MessageTypes.CenterMapOnGeoposition>(this, CenterMapOnLocation);
            Messenger.Default.Unregister<MessageTypes.SetIconState>(this, SetMapIconState);
            base.OnNavigatedFrom(e);
        }

        private async void CenterMapOnLocation(MessageTypes.CenterMapOnGeoposition args)
        {
            await TrySetMapViewWithMargin(args.Position, .01);
        }

        private async Task TrySetMapViewWithMargin(BasicGeoposition pos, double boundingBoxZoomAdjustment, MapAnimationKind mapAnimation = MapAnimationKind.None)
        {
            double narrowZoomAdjustment = boundingBoxZoomAdjustment / 2;

            // Creators update changed maps a bit, so we need to zoom in closer on any device running it.
            // MapStyle is a decent proxy for "are we on Creators or not"
            if (DeviceTypeHelper.GetDeviceFormFactorType() == DeviceFormFactorType.Phone
                && !ApiInformation.IsTypePresent("Windows.UI.Xaml.Controls.Maps.MapStyle"))
            {
                narrowZoomAdjustment = boundingBoxZoomAdjustment / 1.35;
            }

            // Create a box surrounding the specified point to emulate a zoom level on the map.
            // We're using a simulated bounding box, because we need to be able to specify a margin,
            // to accommodate either of the the floating content panels.
            BasicGeoposition northwest = BasicGeopositionExtensions.Create
            (
                0.0,
                pos.Longitude - boundingBoxZoomAdjustment,
                pos.Latitude + boundingBoxZoomAdjustment
            );
            BasicGeoposition southeast = BasicGeopositionExtensions.Create
            (
                0.0,
                pos.Longitude + boundingBoxZoomAdjustment,
                pos.Latitude - boundingBoxZoomAdjustment
            );
            if (AdaptiveVisualStateGroup.CurrentState == _narrowVisualState)
            {
                // Zoom in a little further when in the narrow view, otherwise we're a little
                // too far out for the narrow field of view
                northwest.Longitude += narrowZoomAdjustment;
                northwest.Latitude -= narrowZoomAdjustment;

                southeast.Longitude -= narrowZoomAdjustment;
                southeast.Latitude += narrowZoomAdjustment;

                GeoboundingBox box = new GeoboundingBox(northwest, southeast);
                if (NarrowSearchPanel.IsOpen)
                {
                    double bottomMargin = NarrowSearchPanel.ExpandedHeight;
                    await PageMap.TrySetViewBoundsAsync(box, new Thickness(0, 0, 0, bottomMargin), mapAnimation);
                }
                else
                {
                    await PageMap.TrySetViewBoundsAsync(box, new Thickness(0, 0, 0, 0), mapAnimation);
                }
            }
            else
            {
                GeoboundingBox box = new GeoboundingBox(northwest, southeast);
                await PageMap.TrySetViewBoundsAsync(box, new Thickness(410, 0, 0, 0), mapAnimation);
            }
        }

        private void NarrowSearchPanel_Loaded(object sender, RoutedEventArgs e)
        {
            NarrowSearchPanel.ExpandedHeight = this.ActualHeight * FloatingPanelHeightFraction;
            this.SizeChanged += SearchPage_SizeChanged;            
            if (this.Parent != null)
            {
                (this.Parent as FrameworkElement).SizeChanged += Parent_SizeChanged;
            }
        }

        private void SearchPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NarrowSearchPanel.ExpandedHeight = this.ActualHeight * FloatingPanelHeightFraction;
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

        private void SearchLineSelectionChanged(MessageTypes.SearchLineSelectionChanged _)
        {
            GeoboundingBox lineBox = PageMap.GetAllMapElementsBoundingBox();
            var mapMargin = AdaptiveVisualStateGroup.CurrentState == _narrowVisualState
                ? new Thickness(0, 0, 0, (this.NarrowSearchPanel.ExpandedHeight) + 0)
                : new Thickness(410, 10, 10, 10);
            PageMap.TrySetViewBoundsAsync(lineBox, mapMargin, MapAnimationKind.Linear).DoNotAwait();
        }

        private void NearbyListSelectionChanged(MessageTypes.NearbyListSelectionChanged args)
        {                        
            TrySetMapViewWithMargin(args.SelectedStop.Coords, .005, MapAnimationKind.Linear).DoNotAwait();            
        }

        private void StopsListSelectionChanged(MessageTypes.StopsListSelectionChanged args)
        {            
            TrySetMapViewWithMargin(args.SelectedStop.Coords, .005, MapAnimationKind.Linear).DoNotAwait();            
        }

        private void PageMap_MapRightTapped(MapControl sender, MapRightTappedEventArgs args)
        {
            var flyout = (MenuFlyout)Flyout.GetAttachedFlyout(PageMap);
            ((MenuFlyoutItem)flyout.Items[1]).CommandParameter = args.Location;
            flyout.ShowAt(sender, args.Position);
        }

        private void SetMapIconState(MessageTypes.SetIconState args)
        {
            this.PageMap.SetIconState(args.MapIconId, args.NewState);
        }

        private void PageMap_MapElementClicked(MapControl sender, MapElementClickEventArgs args)
        {
            IEnumerable<Guid> tappedIds = args.MapElements
                .OfType<MapIcon>()
                .Select(x => (Guid)x.GetValue(MapElementExtensions.PoiIdProperty))
                .Where(x => x != default(Guid));

            ViewModel.MapElementTappedCommand.Execute(tappedIds);
        }
    }
}
