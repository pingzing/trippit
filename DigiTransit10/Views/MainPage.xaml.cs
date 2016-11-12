using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Storyboards;
using DigiTransit10.ViewModels;
using DigiTransit10.ViewModels.ControlViewModels;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Template10.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static DigiTransit10.Helpers.MessageTypes;

namespace DigiTransit10.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainViewModel ViewModel => this.DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.AdaptiveVisualStateGroup.CurrentStateChanged += AdaptiveVisualStateGroup_CurrentStateChanged;
            this.Loaded += MainPage_Loaded;

            Messenger.Default.Register<MessageTypes.PlanFoundMessage>(this, PlanFound);
            Messenger.Default.Register<MessageTypes.NavigationCanceled>(this, NavigationCanceledByUser);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            string currentStateName = AdaptiveVisualStateGroup.CurrentState.Name;
            UpdateViewModelVisualState(currentStateName);
        }

        private void AdaptiveVisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            string currentStateName = AdaptiveVisualStateGroup.CurrentState.Name;
            UpdateViewModelVisualState(currentStateName);
        }

        private void UpdateViewModelVisualState(string currentStateName)
        {
            if (currentStateName == Constants.VisualStateNarrow)
            {
                ViewModel.TripFormViewModel.CurrentVisualState = VisualStateType.Narrow;
            }
            else if (currentStateName == Constants.VisualStateNormal)
            {
                ViewModel.TripFormViewModel.CurrentVisualState = VisualStateType.Normal;
            }
            else if (currentStateName == Constants.VisualStateWide)
            {
                ViewModel.TripFormViewModel.CurrentVisualState = VisualStateType.Wide;
            }
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

        private FavoritePlace _pinnedFavoriteClicked;
        private ListView _pinnedFavoritesList;
        private bool _isPlayingExitAnimation;
        private void PinnedFavoritesControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            _pinnedFavoriteClicked = e.ClickedItem as FavoritePlace;
            if(_pinnedFavoritesList == null)
            {
                _pinnedFavoritesList = sender as ListView;
            }
            if (_pinnedFavoriteClicked != null)
            {
                ViewModel?.TripFormViewModel?.FavoritePlaceClickedCommand.Execute(_pinnedFavoriteClicked);
                if(AdaptiveVisualStateGroup.CurrentState.Name == Constants.VisualStateNarrow)
                {
                    _isPlayingExitAnimation = true;
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_pinnedFavoriteClicked != null
                && _pinnedFavoritesList != null
                && _isPlayingExitAnimation)
            {
                //double check to make sure that the PlanTrip actually found a Trip
                if(!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults)
                    || BootStrapper.Current.SessionState[NavParamKeys.PlanResults] == null)
                {
                    _pinnedFavoriteClicked = null;
                    _pinnedFavoritesList = null;
                    _isPlayingExitAnimation = false;
                    return;
                }

                e.Cancel = true;

                var clickedItem = (FrameworkElement)_pinnedFavoritesList.ContainerFromItem(_pinnedFavoriteClicked);
                clickedItem = clickedItem.FindChild<TextBlock>("FavoriteName");
                var storyboard = ContinuumNavigationExitFactory.GetAnimation(clickedItem);
                storyboard.Completed += ExitAnimation_Completed;
                storyboard.Begin();

                this.IsEnabled = false;
                this.MainPageBottomBar.IsEnabled = false;
            }
        }

        private void ExitAnimation_Completed(object sender, object e)
        {
            Storyboard storyboard = (Storyboard)sender;
            storyboard.Completed -= ExitAnimation_Completed;

            _isPlayingExitAnimation = false;

            BootStrapper.Current.NavigationService.Navigate(typeof(TripResultPage));

            this.IsEnabled = true;
            this.MainPageBottomBar.IsEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(_pinnedFavoriteClicked != null
                && _pinnedFavoritesList != null)
            {
                var clickedItem = (FrameworkElement)_pinnedFavoritesList.ContainerFromItem(_pinnedFavoriteClicked);

                _pinnedFavoritesList = null;
                _pinnedFavoriteClicked = null;

                clickedItem = clickedItem.FindChild<TextBlock>("FavoriteName");
                var storyboard = ContinuumNavigationEntranceFactory.GetAnimation(clickedItem);
                storyboard.Begin();
            }
        }

        private void NavigationCanceledByUser(NavigationCanceled _)
        {
            // Prevent hijacking of Navigation in OnNavigatedTo--the user canceled the previous navigation.
            this._pinnedFavoriteClicked = null;
            this._pinnedFavoritesList = null;
        }

        private void PinnedFavoritesControl_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            var tappedItem = (e.OriginalSource as FrameworkElement).DataContext as IFavorite;

            if (item != null)
            {
                MenuFlyout flyout = FlyoutBase.GetAttachedFlyout(item) as MenuFlyout;
                ((MenuFlyoutItem)flyout.Items[0]).CommandParameter = tappedItem;
                flyout.ShowAt(this, e.GetPosition(this));
            }
        }

        private void IntermediatePlaceRemoved_Click(object sender, RoutedEventArgs e)
        {
            var model = ((sender as Button)?.Parent as FrameworkElement)?.DataContext as IntermediateSearchViewModel;
            if(model != null)
            {
                this.Focus(FocusState.Programmatic); //Otherwise, focus jumps to the To box, and that's annoying
                if(ViewModel.TripFormViewModel.RemoveIntermediateCommand.CanExecute(model))
                {
                    ViewModel.TripFormViewModel.RemoveIntermediateCommand.Execute(model);
                }
            }
        }

        private void PlanFound(MessageTypes.PlanFoundMessage obj)
        {
            if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                return;
            }

            if (WideHub != null)
            {
                TripResultTripResultHubSection.Visibility = Visibility.Visible;
                if (WideHub == null || WideHub.Sections.Count < 2)
                {
                    return;
                }
                WideHub.ScrollToSection(WideHub.Sections[1]);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
