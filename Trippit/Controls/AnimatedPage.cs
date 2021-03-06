﻿using System;
using Trippit.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Trippit.Controls
{
    /// <summary>
    /// A page which provides default, overridable animations when navigated to/from.
    /// Note that in order for the animations to run, inheritors must call base.OnNavigatingFrom()
    /// and base.OnNavigatedTo().
    /// </summary>
    public class AnimatedPage : Page
    {
        private Lazy<Storyboard> _showBottomBarStoryboard;
        private Lazy<Storyboard> _defaultToStoryboard;
        private Lazy<Storyboard> _defaultFromStoryboard;
        private Lazy<Storyboard> _hideBottomBarStoryboard;

        private Lazy<PageAnimation> _defaultToAnimation;
        private Lazy<PageAnimation> _defaultFromAnimation;

        public PageAnimation ToAnimation { get; set; }

        /// <summary>
        /// This is the flag that determines whether or not OnNavigating will cancel and wait for
        /// the FromAnimation.
        /// </summary>
        private bool _fromAnimationCompleted;
        private bool _isNavigatingBack = false;
        private Type _navigatingToType = null;
        private object _navigatingToParameter = null;
        public PageAnimation FromAnimation { get; set; }

        public AnimatedPage()
        {            
            _defaultFromStoryboard = new Lazy<Storyboard>(() =>
            {
                Storyboard defaultFromStoryboard = new Storyboard();
                DoubleAnimation fadeAnimation = new DoubleAnimation();
                fadeAnimation.From = 1;
                fadeAnimation.To = 0;
                fadeAnimation.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromMilliseconds(333));
                Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
                Storyboard.SetTarget(fadeAnimation, this);
                defaultFromStoryboard.Children.Add(fadeAnimation);
                return defaultFromStoryboard;
            });

            _hideBottomBarStoryboard = new Lazy<Storyboard>(() =>
            {
                Storyboard hideBottomBarStoryboard = new Storyboard();
                DoubleAnimation hideBottomAnimation = new DoubleAnimation();
                hideBottomAnimation.To = this.BottomAppBar.ActualHeight;
                hideBottomAnimation.EasingFunction = new BackEase
                {
                    Amplitude = 0.3,
                    EasingMode = EasingMode.EaseIn
                };
                hideBottomAnimation.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromMilliseconds(300));
                Storyboard.SetTargetProperty(hideBottomAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
                Storyboard.SetTarget(hideBottomAnimation, this.BottomAppBar);
                hideBottomBarStoryboard.Children.Add(hideBottomAnimation);
                return hideBottomBarStoryboard;
            });

            _defaultToStoryboard = new Lazy<Storyboard>(() =>
            {
                Storyboard defaultToStoryboard = new Storyboard();
                DoubleAnimation fadeAnimation = new DoubleAnimation();
                fadeAnimation.From = 0;
                fadeAnimation.To = 1;
                fadeAnimation.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromMilliseconds(333));
                Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
                Storyboard.SetTarget(fadeAnimation, this);
                defaultToStoryboard.Children.Add(fadeAnimation);
                return defaultToStoryboard;
            });

            _showBottomBarStoryboard = new Lazy<Storyboard>(() =>
            {
                Storyboard showBottomBarStoryboard = new Storyboard();
                DoubleAnimation showBottomAnimation = new DoubleAnimation();
                showBottomAnimation.From = 50; //The bar hasn't rendered yet, so we just have to hope it isn't minimzed or something.
                showBottomAnimation.To = 0;
                showBottomAnimation.EasingFunction = new BackEase
                {
                    Amplitude = 0.3,
                    EasingMode = EasingMode.EaseOut
                };
                showBottomAnimation.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromMilliseconds(600));
                Storyboard.SetTargetProperty(showBottomAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
                Storyboard.SetTarget(showBottomAnimation, this.BottomAppBar);
                showBottomBarStoryboard.Children.Add(showBottomAnimation);
                return showBottomBarStoryboard;
            });

            _defaultToAnimation = new Lazy<PageAnimation>(() => 
            {
                return new PageAnimation
                {
                    ForwardStoryboard = _defaultToStoryboard.Value,
                    BackStoryboard = _defaultToStoryboard.Value
                };
            });

            _defaultFromAnimation = new Lazy<PageAnimation>(() => 
            {
                return new PageAnimation
                {
                    ForwardStoryboard = _defaultFromStoryboard.Value,
                    BackStoryboard = _defaultFromStoryboard.Value
                };
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Storyboard toBoard;
            if (ToAnimation == null)
            {
                toBoard = e.NavigationMode == NavigationMode.Back
                    ? _defaultToAnimation.Value.BackStoryboard
                    : _defaultToAnimation.Value.ForwardStoryboard;
            }
            else
            {
                toBoard = e.NavigationMode == NavigationMode.Back
                    ? ToAnimation.BackStoryboard
                    : ToAnimation.ForwardStoryboard;
            }

            if (toBoard != null)
            {
                toBoard.Completed -= ToAnimation_Completed;
                toBoard.Completed += ToAnimation_Completed;
                if (this.BottomAppBar != null)
                {
                    this.BottomAppBar.IsEnabled = false;                    
                    _showBottomBarStoryboard.Value.Begin();
                }
                toBoard.Begin();
            }
            base.OnNavigatedTo(e);
        }

        private void ToAnimation_Completed(object sender, object e)
        {
            if (this.BottomAppBar != null)
            {
                this.BottomAppBar.IsEnabled = true;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (!_fromAnimationCompleted)
            {
                Storyboard fromBoard;
                if (FromAnimation == null)
                {
                    fromBoard = e.NavigationMode == NavigationMode.Back
                        ? _defaultFromAnimation.Value.BackStoryboard
                        : _defaultFromAnimation.Value.ForwardStoryboard;
                }
                else
                {
                    fromBoard = e.NavigationMode == NavigationMode.Back
                        ? FromAnimation.BackStoryboard
                        : FromAnimation.ForwardStoryboard;
                }

                e.Cancel = true;
                fromBoard.Completed -= FromAnimation_Completed;
                fromBoard.Completed += FromAnimation_Completed;
                _navigatingToType = e.SourcePageType;
                _navigatingToParameter = e.Parameter;
                _isNavigatingBack = e.NavigationMode == NavigationMode.Back;

                if (this.BottomAppBar != null)
                {
                    this.BottomAppBar.IsEnabled = false;
                }

                if (this.BottomAppBar != null)
                {
                    _hideBottomBarStoryboard.Value.Begin();
                }
                fromBoard.Begin();
            }
            else
            {
                _fromAnimationCompleted = false;
            }
            base.OnNavigatingFrom(e);
        }

        private void FromAnimation_Completed(object sender, object e)
        {
            _fromAnimationCompleted = true;
            if (_isNavigatingBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(_navigatingToType, _navigatingToParameter);
            }

            _isNavigatingBack = false;
            _navigatingToType = null;
            _navigatingToParameter = null;
        }
    }
}
