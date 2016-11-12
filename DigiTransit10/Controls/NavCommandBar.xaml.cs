using DigiTransit10.Helpers;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Template10.Services.NavigationService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DigiTransit10.Controls
{
    public sealed partial class NavCommandBar : CommandBar, INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private AppBarButton _currentlySelected = null;
        private Dictionary<ICommandBarElement, long> _visibilityTrackedElements = new Dictionary<ICommandBarElement, long>();
        DispatcherTimer _visibilityChangedThrottle = new DispatcherTimer();

        public RelayCommand<Type> NavigateCommand => new RelayCommand<Type>(Navigate);

        public NavCommandBar()
        {
            this.InitializeComponent();
            if(ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.CommandBar", "OverflowButtonVisibility"))
            {
                this.OverflowButtonVisibility = CommandBarOverflowButtonVisibility.Visible;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            Views.Busy.BusyChanged += BusyView_BusyChanged;

            _navigationService = Template10.Common.BootStrapper.Current.NavigationService;
            _navigationService.Frame.Navigated += Frame_Navigated;
            this.Loaded += NavCommandBar_Loaded;
            this.Unloaded += NavCommandBar_Unloaded;

            /* AppBarButtons displayed in the NavigationButtons StackPanel won't have their Label
             * Visibility updated automatically when the AppBar opens. So instead, we listen directly 
             * to the IsOpen property, and when it changes, we update each button's IsCompact property 
             * accordingly. */
            this.RegisterPropertyChangedCallback(IsOpenProperty, new DependencyPropertyChangedCallback(IsOpenChanged));
            this.SizeChanged += NavCommandBar_SizeChanged;
            this.PrimaryCommands.VectorChanged += PrimaryCommands_VectorChanged;
            this.SecondaryCommands.VectorChanged += SecondaryCommands_VectorChanged;

            /*Arbitary value. I figure with a target of 60FPS, it's not unreasonabe to expect 
             * simultaneous visibility changes to propagate within ~20 frames of each other. */
            _visibilityChangedThrottle.Interval = TimeSpan.FromMilliseconds(5.34);
            _visibilityChangedThrottle.Tick += (s, args) =>
            {
                ReflowCommands(RenderSize, RenderSize);
                _visibilityChangedThrottle.Stop();
            };
        }

        private void IsOpenChanged(DependencyObject sender, DependencyProperty dp)
        {
            NavCommandBar _this = sender as NavCommandBar;
            bool isOpen = (bool)_this.GetValue(dp);
            _this.UpdateButtonLabels(isOpen);
        }

        private void NavCommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateNavSeparatorVisibility();
            this.UpdateLayout();

            ReflowCommands(this.RenderSize, this.RenderSize);
            UpdateSelectionVisual();

            UpdateButtonLabels(IsOpen);
            UpdateCommandsVisibilityTracking(this.PrimaryCommands);
            UpdateCommandsVisibilityTracking(this.SecondaryCommands);

            if (DeviceTypeHelper.GetDeviceFormFactorType() == DeviceFormFactorType.Phone)
            {
                InputPane.GetForCurrentView().Showing += InputPane_Showing;
                InputPane.GetForCurrentView().Hiding += InputPane_Hiding;
            }
        }

        private void NavCommandBar_Unloaded(object sender, RoutedEventArgs e)
        {
            Views.Busy.BusyChanged -= BusyView_BusyChanged;
            if (DeviceTypeHelper.GetDeviceFormFactorType() == DeviceFormFactorType.Phone)
            {
                InputPane.GetForCurrentView().Showing -= InputPane_Showing;
                InputPane.GetForCurrentView().Hiding -= InputPane_Hiding;
            }
        }

        //---Explanation
        /* This is a weird one. If a page has a BottomBar, it stays stuck to the top
         * of the keyboard, should the software keyboard be shown on a phone. Unfortunately, it
         * fails to do a layout check to see if the focused textbox is occluded by the 
         * app bar. So, we work around it by just hiding the damn bar when the keyboard 
         * is visible.
         * Also, we're using Opacity instead of Visibility so a.) we don't trigger a 
         * relayout and b.) so we don't stomp all over our visual states.*/
        private void InputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            this.Opacity = 0;
            this.IsHitTestVisible = false;
        }

        private void InputPane_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            this.Opacity = 1;
            this.IsHitTestVisible = true;
        }
        //---end explanation

        private void NavCommandBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReflowCommands(e.PreviousSize, e.NewSize);
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            UpdateSelectionVisual();
            UpdateButtonLabels(IsOpen);
        }

        private void PrimaryCommands_VectorChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs evt)
        {
            System.Diagnostics.Debug.WriteLine("Primary vector changed, " + sender.Count + " elements in sender");
            UpdateCommandsVisibilityTracking(sender);
            UpdateNavSeparatorVisibility();
        }

        private void SecondaryCommands_VectorChanged(IObservableVector<ICommandBarElement> sender, IVectorChangedEventArgs @event)
        {
            System.Diagnostics.Debug.WriteLine("Secondary vector changed, " + sender.Count + " elements in sender");
            UpdateCommandsVisibilityTracking(sender);
        }

        private void UpdateCommandsVisibilityTracking(IEnumerable<ICommandBarElement> sender)
        {
            List<ICommandBarElement> toRemove = new List<ICommandBarElement>();
            foreach (KeyValuePair<ICommandBarElement, long> element in _visibilityTrackedElements)
            {
                var visibilityTrackable = element.Key as FrameworkElement;
                if (visibilityTrackable != null
                    && !(PrimaryCommands.Contains(element.Key) || SecondaryCommands.Contains(element.Key)))
                {
                    visibilityTrackable.UnregisterPropertyChangedCallback(VisibilityProperty, element.Value);
                }
            }

            foreach (var stale in toRemove)
            {
                _visibilityTrackedElements.Remove(stale);
            }

            foreach (ICommandBarElement element in sender)
            {
                if(element is NavAppBarButton)
                {
                    continue;
                }
                var visibilityTrackable = element as FrameworkElement;
                if (visibilityTrackable != null && !_visibilityTrackedElements.ContainsKey(element))
                {
                    long callbackToken = visibilityTrackable.RegisterPropertyChangedCallback(VisibilityProperty, OnCommandBarElementVisibilityChanged);
                    _visibilityTrackedElements.Add(element, callbackToken);
                }
            }
        }

        private void OnCommandBarElementVisibilityChanged(DependencyObject sender, DependencyProperty dp)
        {
            /* We don't want to reflow immediately--if several buttons change visibility at once, the 
             * various Reflow calls end up stepping all over each others' toes. So instead, we've got 
             * a tiny timer (~5ms) that batches our Reflow commands together. The actual Reflow
             command happens in the Timer's .Tick event.*/
            _visibilityChangedThrottle.Stop();
            _visibilityChangedThrottle.Start();
        }

        //Disable the bar when we have a Loading/Busy overlay visible.
        private void BusyView_BusyChanged(object sender, bool newIsBusy)
        {
            this.IsEnabled = !newIsBusy;
        }

        private void ReflowCommands(Size oldSize, Size newSize)
        {
            //Always leave Home, the currently selected page, and the AppBarSeparator in the Nav panel
            int navElementsToKeep = (_currentlySelected == null || _currentlySelected == HomeButton)
                ? 2
                : 3;

            double ellipsisButtonWidth = 48;
            double separatorWidth = 32;
            double appButtonWidth = HomeButton.ActualWidth; //we just need the width of any old AppBarButton here, so we're using one that's readily available
            var currWidth = this.ActualWidth;
            var navWidth = this.NavigationButtons.ActualWidth;
            //Only factor in visible elements for width calculations
            double primaryCommandsWidth = this.PrimaryCommands
                .OfType<UIElement>()
                .Count(x => x.Visibility == Visibility.Visible) * appButtonWidth + ellipsisButtonWidth;

            //shrinking or staying the same
            if (newSize.Width <= oldSize.Width)
            {
                while ((navWidth + primaryCommandsWidth) > currWidth) //reflow is necessary
                {
                    if (this.NavigationButtons.Children.Count > navElementsToKeep)
                    {
                        //Leave the current page, otherwise grab the element with the highest-numbered position.
                        var buttonToMove = (NavAppBarButton)this.NavigationButtons.Children
                            .Where(x => x != _currentlySelected && !(x is AppBarSeparator))
                            .Max(x => x as ISortableAppBarButton);

                        this.NavigationButtons.Children.Remove(buttonToMove);
                        InsertToSecondaryBar(buttonToMove);
                        navWidth -= appButtonWidth;
                    }
                    else //no more space, start removing PrimaryCommands
                    {
                        var buttonToMove = (ICommandBarElement)this.PrimaryCommands.Max(x => x as ISortableAppBarButton);
                        if (buttonToMove == null)
                        {
                            break;
                        }
                        this.PrimaryCommands.Remove(buttonToMove);
                        InsertToSecondaryBar(buttonToMove);
                        primaryCommandsWidth -= appButtonWidth;
                    }
                }
            }

            //growing or staying the same
            if (newSize.Width >= oldSize.Width)
            {
                while (currWidth - navWidth - primaryCommandsWidth >= appButtonWidth
                    && this.SecondaryCommands.Count > 0) //the bar has space for at least one button, and there are buttons to add
                {
                    //If we're adding back the first PrimaryCommand, also factor in the width of the separator
                    if(PrimaryCommands.Count == 0
                        && currWidth - navWidth - primaryCommandsWidth - separatorWidth < appButtonWidth)
                    {
                        return;
                    }

                    //start by adding by PrimaryCommands
                    var barButton = (ICommandBarElement)this.SecondaryCommands
                        .Where(x => !(x is NavAppBarButton))
                        .Min(x => x as ISortableAppBarButton);

                    if (barButton != null)
                    {
                        this.SecondaryCommands.Remove(barButton);
                        //Insert to Primary bar
                        this.PrimaryCommands.Add(barButton);
                        ((ISortableAppBarButton)barButton).IsSecondaryCommand = false;
                        TryRemoveSecondarySeparator();

                        primaryCommandsWidth += appButtonWidth;
                    }
                    else
                    {
                        //After all those are back, start adding NavCommands
                        var navCommand = (NavAppBarButton)this.SecondaryCommands
                            .Where(x => x is NavAppBarButton)
                            .Min(x => x as ISortableAppBarButton);

                        if (navCommand != null)
                        {
                            this.SecondaryCommands.Remove(navCommand);
                            //Insert to Nav Bar
                            int navButtonsCount = this.NavigationButtons.Children.Count;
                            this.NavigationButtons.Children.Insert(navButtonsCount - 1, navCommand);
                            navCommand.IsSecondaryCommand = false;
                            navCommand.IsEnabled = true;
                            TryRemoveSecondarySeparator();
                            UpdateButtonLabels(IsOpen);

                            navWidth += appButtonWidth;
                        }
                    }
                }
            }
        }

        private void InsertToSecondaryBar(NavAppBarButton buttonToMove)
        {
            buttonToMove.IsSecondaryCommand = true;

            if (SecondaryCommands.Any(x => x is MovableAppBarButton) && !SecondaryCommands.Any(x => x is AppBarSeparator))
            {
                SecondaryCommands.Insert(0, new AppBarSeparator());
            }

            int separatorIndex = -1;
            if (SecondaryCommands.Any(x => x is AppBarSeparator))
            {
                separatorIndex = SecondaryCommands.IndexOf(SecondaryCommands.First(x => x is AppBarSeparator));
            }

            int endIndex = separatorIndex != -1 ? separatorIndex - 1 : SecondaryCommands.Count - 1;
            if (endIndex == -1) //nothing in the SecondaryCommands list yet
            {
                SecondaryCommands.Add(buttonToMove);
            }
            else //we've got at least one other SecondaryCommand, maybe more
            {
                for (int i = 0; i <= endIndex; i++)
                {
                    int currCommandPosition = ((ISortableAppBarButton)SecondaryCommands[i]).Position;
                    if (buttonToMove.Position < currCommandPosition)
                    {
                        SecondaryCommands.Insert(i, buttonToMove);
                        return;
                    }
                }
                //if we get through the entire list and we don't find anything of a lesser priority, the new button belongs at the end
                SecondaryCommands.Insert(endIndex, buttonToMove);
            }
        }

        private void InsertToSecondaryBar(ICommandBarElement buttonToMove)
        {
            ((ISortableAppBarButton)buttonToMove).IsSecondaryCommand = true;
            if (SecondaryCommands.Any(x => x is NavAppBarButton) && !SecondaryCommands.Any(x => x is AppBarSeparator))
            {
                SecondaryCommands.Add(new AppBarSeparator());
            }

            int separatorIndex = -1;
            if (SecondaryCommands.Any(x => x is AppBarSeparator))
            {
                separatorIndex = SecondaryCommands.IndexOf(SecondaryCommands.First(x => x is AppBarSeparator));
            }

            int startIndex = separatorIndex != -1 ? separatorIndex + 1 : 0;
            if (startIndex == 0)
            {
                SecondaryCommands.Add(buttonToMove);
            }
            else
            {
                for (int i = startIndex; i < SecondaryCommands.Count; i++)
                {
                    int currCommandPosition = ((ISortableAppBarButton)SecondaryCommands[i]).Position;
                    if (((ISortableAppBarButton)buttonToMove).Position < currCommandPosition)
                    {
                        SecondaryCommands.Insert(i, buttonToMove);
                        return;
                    }
                }
                //if we get through the entire list and we don't find anything of a lesser priority, the new button belongs at the end
                SecondaryCommands.Add(buttonToMove);
            }
        }

        private void TryRemoveSecondarySeparator()
        {
            if (SecondaryCommands.Any(x => x is AppBarSeparator) && (
                    SecondaryCommands.Count == 0
                    || !SecondaryCommands.Any(x => x is NavAppBarButton)
                    || !SecondaryCommands.Any(x => x is ISortableAppBarButton)
                ))
            {
                int separatorIndex = SecondaryCommands.IndexOf(SecondaryCommands.First(x => x is AppBarSeparator));
                SecondaryCommands.RemoveAt(separatorIndex);
            }
        }

        private void UpdateSelectionVisual()
        {
            HomeButton.IsSelected = false;
            FavoritesButton.IsSelected = false;
            SearchButton.IsSelected = false;
            SettingsButton.IsSelected = false;

            if (_navigationService.CurrentPageType == typeof(Views.MainPage))
            {
                HomeButton.IsSelected = true;
                _currentlySelected = HomeButton;
            }
            if (_navigationService.CurrentPageType == typeof(Views.FavoritesPage))
            {
                FavoritesButton.IsSelected = true;
                _currentlySelected = FavoritesButton;
            }
            if(_navigationService.CurrentPageType == typeof(Views.SearchPage))
            {
                SearchButton.IsSelected = true;
                _currentlySelected = SearchButton;
            }
            if(_navigationService.CurrentPageType == typeof(Views.SettingsPage))
            {
                SettingsButton.IsSelected = true;
                _currentlySelected = SettingsButton;
            }
        }

        private void UpdateButtonLabels(bool isOpen)
        {
            foreach (var button in NavigationButtons.Children.OfType<NavAppBarButton>())
            {
                button.IsCompact = !isOpen;
            }
        }

        private void UpdateNavSeparatorVisibility()
        {
            if (PrimaryCommands.Count > 0)
            {
                if (NavButtonSeparator.Visibility == Visibility.Collapsed)
                {
                    NavButtonSeparator.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if(NavButtonSeparator.Visibility == Visibility.Visible)
                {
                    NavButtonSeparator.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Navigate(Type destination)
        {
            _navigationService.Frame.Navigated -= Frame_Navigated;
            if(destination == typeof(Views.MainPage))
            {
                _navigationService.ClearHistory();
            }
            _navigationService.NavigateAsync(destination);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
