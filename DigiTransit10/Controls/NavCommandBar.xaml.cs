using System;
using GalaSoft.MvvmLight.Command;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using System.Linq;
using Windows.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace DigiTransit10.Controls
{
    public sealed partial class NavCommandBar : CommandBar, INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private AppBarButton _currentlySelected = null;        

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
           
            _navigationService = App.Current.NavigationService;
            _navigationService.Frame.Navigated += Frame_Navigated;
            this.Loaded += NavCommandBar_Loaded;
            this.PrimaryCommands.VectorChanged += PrimaryCommands_VectorChanged;
            
            /* AppBarButtons displayed in the NavigationButtons StackPanel won't have their Label
             * Visibility updated automatically when the AppBar opens. Doing it via binding is bizarrely 
             * unreliable. So instead, we listen directly to the IsOpen property, and when it changes, 
             * we update each button's IsCompact property accordingly. 
             */
            this.RegisterPropertyChangedCallback(IsOpenProperty, new DependencyPropertyChangedCallback(IsOpenChanged));
            this.SizeChanged += NavCommandBar_SizeChanged;
        }

        private void IsOpenChanged(DependencyObject sender, DependencyProperty dp)
        {
            NavCommandBar _this = sender as NavCommandBar;
            bool isOpen = (bool)_this.GetValue(dp);
            _this.UpdateButtonLabels(isOpen);           
        }

        private void NavCommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            var currSize = new Size(this.ActualWidth, this.ActualHeight);
            UpdateNavSeparatorVisibility();
            this.UpdateLayout();            

            ReflowCommands(currSize, currSize);
            UpdateSelectionVisual();
            
            UpdateButtonLabels(IsOpen);                        
        }

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
            UpdateNavSeparatorVisibility();
        }

        //Disable the bar when we have a Loading/Busy overlay visible.
        private void BusyView_BusyChanged(object sender, bool newIsBusy)
        {
            this.IsEnabled = !newIsBusy;
        }

        private void ReflowCommands(Size oldSize, Size newSize)
        {
            double ellipsisButtonWidth = 48;
            double separatorWidth = 32;
            double appButtonWidth = HomeButton.ActualWidth; //we just need the width of any old AppBarButton here, so we're using one that's readily available
            var currWidth = this.ActualWidth;
            var navWidth = this.NavigationButtons.ActualWidth;
            double primaryCommandsWidth = 0;
            foreach (var cmd in this.PrimaryCommands)
            {
                primaryCommandsWidth += appButtonWidth;
            }            

            primaryCommandsWidth += ellipsisButtonWidth;            

            if (newSize.Width <= oldSize.Width)
            {
                //shrinking
                while ((navWidth + primaryCommandsWidth) > currWidth) //reflow is necessary
                {
                    if (this.NavigationButtons.Children.Count > 3) //always leave 1.) Home, 2.) Current Page 3.) The AppBarSeparator
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
                        var buttonToMove = (MovableAppBarButton)this.PrimaryCommands.Max(x => x as ISortableAppBarButton);
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
            else
            {
                //growing
                while (currWidth - navWidth - primaryCommandsWidth >= appButtonWidth
                    && this.SecondaryCommands.Count > 0) //the bar has space for at least one button, and there are buttons to add
                {
                    //If we're adding back the first PrimaryCommand, factor in the width of the separator we'll be adding too
                    if(PrimaryCommands.Count == 0
                        && currWidth - navWidth - primaryCommandsWidth - separatorWidth < appButtonWidth)
                    {
                        return;                        
                    }

                    //start by adding by PrimaryCommands
                    var primaryCommand = (MovableAppBarButton)this.SecondaryCommands
                        .Where(x => x is MovableAppBarButton)
                        .Min(x => x as ISortableAppBarButton);

                    if (primaryCommand != null)
                    {
                        this.SecondaryCommands.Remove(primaryCommand);
                        InsertToPrimaryBar(primaryCommand);
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
                            InsertToNavBar(navCommand);
                            navWidth += appButtonWidth;
                        }
                    }
                }
            }            
        }

        private void InsertToNavBar(NavAppBarButton navCommand)
        {
            int navButtonsCount = this.NavigationButtons.Children.Count;            
            this.NavigationButtons.Children.Insert(navButtonsCount - 1, navCommand);
            navCommand.IsSecondaryCommand = false;
            navCommand.IsEnabled = true;
                
            TryRemoveSecondarySeparator();
            UpdateButtonLabels(IsOpen);
        }

        private void InsertToPrimaryBar(MovableAppBarButton primaryCommand)
        {
            this.PrimaryCommands.Add(primaryCommand);
            primaryCommand.IsEnabled = true;
            primaryCommand.IsSecondaryCommand = false;

            TryRemoveSecondarySeparator();
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

        private void InsertToSecondaryBar(MovableAppBarButton buttonToMove)
        {
            buttonToMove.IsSecondaryCommand = true;
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
                    if (buttonToMove.Position < currCommandPosition)
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
                    || !SecondaryCommands.Any(x => x is MovableAppBarButton)
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
        }

        private void UpdateButtonLabels(bool isOpen)
        {
            foreach (var button in NavigationButtons.Children.Where(x => x is NavAppBarButton))
            {
                ((NavAppBarButton)button).IsCompact = !isOpen;
                string label = ((NavAppBarButton) button).Label;
                ((NavAppBarButton) button).Label = "";
                ((NavAppBarButton) button).Label = label;
            }            
        }

        private void UpdateNavSeparatorVisibility()
        {
            if (PrimaryCommands.Count > 0
                && NavButtonSeparator.Visibility == Visibility.Collapsed)
            {
                NavButtonSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                if (NavButtonSeparator.Visibility == Visibility.Visible)
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
