using System;
using GalaSoft.MvvmLight.Command;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using System.Linq;
using Windows.UI.Xaml;

namespace DigiTransit10.Controls
{
    public sealed partial class NavCommandBar : CommandBar
    {
        private readonly INavigationService _navigationService;
        private AppBarButton _currentlySelected = null;
        private const string NavCommandTag = "NavCommandButton";
        private const string PrimaryCommandTag = "PrimaryCommand";

        public RelayCommand HomeCommand => new RelayCommand(GoHome);
        public RelayCommand FavoritesCommand => new RelayCommand(GoFavorites);

        public NavCommandBar()
        {
            this.InitializeComponent();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            Views.Busy.BusyChanged += BusyView_BusyChanged;
           
            _navigationService = App.Current.NavigationService;
            _navigationService.Frame.Navigated += Frame_Navigated;
            this.Loaded += NavCommandBar_Loaded;            
        }

        private void NavCommandBar_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var currSize = new Size(this.ActualWidth, this.ActualHeight);
            ReflowCommands(currSize, currSize);
            this.SizeChanged += NavCommandBar_SizeChanged;
        }

        private void NavCommandBar_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {            
            ReflowCommands(e.PreviousSize, e.NewSize);
        }

        //todo: clean this up, refactor this to use subclasses of AppBarButton instead of just hacking things together with Tags
        private void ReflowCommands(Size oldSize, Size newSize)
        {
            double appButtonWidth = HomeButton.ActualWidth; //we just need the width of any old AppBarButton here, so we're using one that's readily available
            var currWidth = this.ActualWidth;
            var navWidth = this.NavigationButtons.ActualWidth;
            double primaryCommandsWidth = 0;
            foreach(var cmd in this.PrimaryCommands)
            {
                primaryCommandsWidth += appButtonWidth;
            }
            primaryCommandsWidth += 48; //accounting for the Ellipsis button, which is 48 wide

            if(newSize.Width <= oldSize.Width)
            {
                //shrinking
                while((navWidth + primaryCommandsWidth) > currWidth) //reflow is necessary
                {                    
                    if(this.NavigationButtons.Children.Count > 3) //we can still remove some NavButtons
                    {
                        var buttonToMove = (AppBarButton)this.NavigationButtons.Children
                            .Last(x => x != _currentlySelected && !(x is AppBarSeparator));

                        this.NavigationButtons.Children.Remove(buttonToMove);
                        buttonToMove.Tag = NavCommandTag;
                        this.SecondaryCommands.Add(buttonToMove);
                        navWidth -= appButtonWidth;
                    }
                    else //no more space, start removing PrimaryCommands
                    {
                        AppBarButton buttonToMove = (AppBarButton)this.PrimaryCommands.LastOrDefault();
                        if(buttonToMove == null)
                        {
                            break;
                        }
                        buttonToMove.Tag = PrimaryCommandTag;
                        this.PrimaryCommands.Remove(buttonToMove);
                        this.SecondaryCommands.Add(buttonToMove);
                        primaryCommandsWidth -= appButtonWidth;
                    }
                }
            }
            else
            {
                while(currWidth - navWidth - primaryCommandsWidth >= appButtonWidth 
                    && this.SecondaryCommands.Count > 0) //the bar has space for at least one button, and there are buttons to add
                {
                    //start by adding by PrimaryCommands
                    var primaryCommand = (AppBarButton)this.SecondaryCommands
                        .FirstOrDefault(x => x is AppBarButton && (string)((AppBarButton)x).Tag == PrimaryCommandTag);
                    if (primaryCommand != null)
                    {
                        this.SecondaryCommands.Remove(primaryCommand);
                        this.PrimaryCommands.Add(primaryCommand);
                        primaryCommand.IsEnabled = true;
                        primaryCommandsWidth += appButtonWidth;
                    }
                    else
                    {
                        //After all those are back, start adding NavCommands
                        var navCommand = (AppBarButton)this.SecondaryCommands
                            .FirstOrDefault(x => x is AppBarButton && (string)((AppBarButton)x).Tag == NavCommandTag);
                        if (navCommand != null)
                        {
                            this.SecondaryCommands.Remove(navCommand);
                            navCommand.Tag = false;
                            int navButtonsCount = this.NavigationButtons.Children.Count;
                            //insert at i = -2 because the last element is the AppBarSeparator
                            this.NavigationButtons.Children.Insert(navButtonsCount - 1, navCommand);
                            navCommand.IsEnabled = true;
                            navWidth += appButtonWidth;
                        }
                    }
                }
            }
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {            
            HomeButton.Tag = false;
            FavoritesButton.Tag = false;

            if(_navigationService.CurrentPageType == typeof(Views.MainPage))
            {
                HomeButton.Tag = true;
                _currentlySelected = HomeButton;
            }
            if(_navigationService.CurrentPageType == typeof(Views.FavoritesPage))
            {
                FavoritesButton.Tag = true;
                _currentlySelected = FavoritesButton;
            }

        }

        private void BusyView_BusyChanged(object sender, bool newIsBusy)
        {
            this.IsEnabled = !newIsBusy;
        }

        private void GoHome()
        {
            _navigationService.Frame.Navigated -= Frame_Navigated;
            _navigationService.ClearHistory();
            _navigationService.NavigateAsync(typeof(Views.MainPage));
        }

        private void GoFavorites()
        {
            _navigationService.Frame.Navigated -= Frame_Navigated;
            _navigationService.NavigateAsync(typeof(Views.FavoritesPage));
        }
    }
}
