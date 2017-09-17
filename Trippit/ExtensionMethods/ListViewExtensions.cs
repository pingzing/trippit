using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Trippit.ExtensionMethods
{
    public static class Helpers
    {
        public static ScrollViewer GetScrollViewer(this DependencyObject element)
        {
            if (element is ScrollViewer)
            {
                return (ScrollViewer)element;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }

        public static async Task SmoothScrollIntoViewAsync(
            this ListViewBase listViewBase,
            object dataItem,
            ItemPosition itemPosition = ItemPosition.Default,
            bool disableAnimation = false)
        {
            bool isVirtualizing = false;
            double previousXOffset = default(double);
            double previousYOffset = default(double);

            ScrollViewer scrollViewer = listViewBase.GetScrollViewer();
            var selectorItem = listViewBase.ContainerFromItem(dataItem) as SelectorItem;

            if (selectorItem == null)
            {
                isVirtualizing = true;

                previousXOffset = scrollViewer.HorizontalOffset;
                previousYOffset = scrollViewer.VerticalOffset;

                var tcs = new TaskCompletionSource<bool>();
                void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs args)
                {
                    scrollViewer.ViewChanged -= OnViewChanged;
                    tcs.SetResult(true);
                }
                
                scrollViewer.ViewChanged += OnViewChanged;
                listViewBase.ScrollIntoView(dataItem, ScrollIntoViewAlignment.Leading);
                await tcs.Task;               

                selectorItem = (SelectorItem)listViewBase.ContainerFromItem(dataItem);
            }

            await ScrollIntoView(listViewBase,
                scrollViewer,
                selectorItem,
                isVirtualizing,
                previousXOffset,
                previousYOffset,
                itemPosition,
                disableAnimation);
        }

        public static async Task SmoothScrollIntoViewAsync(
            this ListViewBase listViewBase, 
            int index, 
            ItemPosition itemPosition = ItemPosition.Default, 
            bool disableAnimation = false)
        {
            bool isVirtualizing = false;
            double previousXOffset = default(double);
            double previousYOffset = default(double);

            ScrollViewer scrollViewer = listViewBase.GetScrollViewer();
            var selectorItem = listViewBase.ContainerFromIndex(index) as SelectorItem;

            if (selectorItem == null)
            {
                isVirtualizing = true;

                previousXOffset = scrollViewer.HorizontalOffset;
                previousYOffset = scrollViewer.VerticalOffset;

                var tcs = new TaskCompletionSource<bool>();
                void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs args)
                {
                    scrollViewer.ViewChanged -= OnViewChanged;
                    tcs.SetResult(true);
                }
                
                scrollViewer.ViewChanged += OnViewChanged;
                listViewBase.ScrollIntoView(listViewBase.Items[index], ScrollIntoViewAlignment.Leading);
                await tcs.Task;                                                                

                selectorItem = (SelectorItem)listViewBase.ContainerFromIndex(index);                
            }

            await ScrollIntoView(listViewBase,
                scrollViewer,
                selectorItem,
                isVirtualizing,
                previousXOffset,
                previousYOffset,
                itemPosition,
                disableAnimation);
        }

        private static async Task ScrollIntoView(
            ListViewBase listViewBase,
            ScrollViewer scrollViewer, 
            SelectorItem selectorItem,            
            bool isVirtualizing,
            double previousXOffset,
            double previousYOffset,
            ItemPosition itemPosition = ItemPosition.Default, 
            bool disableAnimation = false)
        {
            var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            if (isVirtualizing)
            {
                var tcs = new TaskCompletionSource<object>();

                EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(null);
                try
                {
                    scrollViewer.ViewChanged += viewChanged;
                    scrollViewer.ChangeView(previousXOffset, previousYOffset, null, true);
                    await tcs.Task;
                }
                finally
                {
                    scrollViewer.ViewChanged -= viewChanged;
                }
            }

            var listViewBaseWidth = listViewBase.ActualWidth;
            var selectorItemWidth = selectorItem.ActualWidth;
            var listViewBaseHeight = listViewBase.ActualHeight;
            var selectorItemHeight = selectorItem.ActualHeight;
            previousXOffset = scrollViewer.HorizontalOffset;
            previousYOffset = scrollViewer.VerticalOffset;

            if (itemPosition == ItemPosition.Left)
            {
                scrollViewer.ChangeView(position.X, previousYOffset, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Top)
            {
                scrollViewer.ChangeView(previousXOffset, position.Y, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Center)
            {
                var CentreX = (listViewBaseWidth - selectorItemWidth) / 2.0;
                var CentreY = (listViewBaseHeight - selectorItemHeight) / 2.0;
                var finalXPosition = position.X - CentreX;
                var finalYPosition = position.Y - CentreY;
                scrollViewer.ChangeView(finalXPosition, finalYPosition, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Right)
            {
                var finalXPosition = position.X - listViewBaseWidth + selectorItemWidth;
                scrollViewer.ChangeView(finalXPosition, previousYOffset, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Bottom)
            {
                var finalYPosition = position.Y - listViewBaseHeight + selectorItemHeight;
                scrollViewer.ChangeView(previousXOffset, finalYPosition, null, disableAnimation);
            }
            else if (itemPosition == ItemPosition.Default)
            {
                var bottomXPosition = position.X - listViewBaseWidth + selectorItemWidth;
                var finalXPosition = position.X;

                if (previousXOffset < position.X && previousXOffset > bottomXPosition)
                {
                    finalXPosition = previousXOffset;
                }
                else if (Math.Abs(previousXOffset - bottomXPosition) < Math.Abs(previousXOffset - position.X))
                {
                    finalXPosition = bottomXPosition;
                }

                var rightYPosition = position.Y - listViewBaseHeight + selectorItemHeight;
                var finalYPosition = position.Y;

                if (previousYOffset < position.Y && previousYOffset > rightYPosition)
                {
                    finalYPosition = previousYOffset;
                }
                else if (Math.Abs(previousYOffset - rightYPosition) < Math.Abs(previousYOffset - position.Y))
                {
                    finalYPosition = rightYPosition;
                }

                scrollViewer.ChangeView(finalXPosition, finalYPosition, null, disableAnimation);
            }
        }

        public static void SmoothScrollNavigation(this ListViewBase listViewBase, int scrollAmount, ScrollNavigationDirection scrollNavigationDirection, bool disableAnimation = false)
        {
            var scrollViewer = listViewBase.GetScrollViewer();

            if (scrollNavigationDirection == ScrollNavigationDirection.Left)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset - scrollAmount, scrollViewer.VerticalOffset, null, disableAnimation);
            }
            else if (scrollNavigationDirection == ScrollNavigationDirection.Up)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset - scrollAmount, null, disableAnimation);
            }
            else if (scrollNavigationDirection == ScrollNavigationDirection.Right)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset + scrollAmount, scrollViewer.VerticalOffset, null, disableAnimation);
            }
            else if (scrollNavigationDirection == ScrollNavigationDirection.Down)
            {
                scrollViewer.ChangeView(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset + scrollAmount, null, disableAnimation);
            }
        }
    }

    /// <summary>
    /// Item Position
    /// </summary>
    public enum ItemPosition
    {
        /// <summary>
        /// If visible then it will not scroll, if not then item will be aligned to the nearest edge
        /// </summary>        
        Default,

        /// <summary>
        /// Aligned left
        /// </summary>        
        Left,

        /// <summary>
        /// Aligned top
        /// </summary>        
        Top,

        /// <summary>
        /// Aligned centre
        /// </summary>        
        Center,

        /// <summary>
        /// Aligned right
        /// </summary>        
        Right,

        /// <summary>
        /// Aligned bottom
        /// </summary>        
        Bottom
    }

    /// <summary>
    /// Scroll Direction
    /// </summary>
    public enum ScrollNavigationDirection
    {
        /// <summary>
        /// Scroll left
        /// </summary>        
        Left,

        /// <summary>
        /// Scroll up
        /// </summary>        
        Up,

        /// <summary>
        /// Scroll right
        /// </summary>        
        Right,

        /// <summary>
        /// Scroll down
        /// </summary>        
        Down,
    }
}
