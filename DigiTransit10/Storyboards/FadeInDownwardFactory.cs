using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace DigiTransit10.Storyboards
{
    /// <summary>
    /// A helper class for creating FadeInDownward storyboard animations.
    /// </summary>
    public static class FadeInDownwardFactory
    {                 
        /// <summary>
        /// Returns a storyboard object that applies a gentle fade-and-downward-swoop to the input element.
        /// </summary>
        /// <param name="target">The object to apply the storyboard animation to.</param>
        /// <returns>A <see cref="Storyboard"/> that defines the animation.</returns>
        public static Storyboard GetAnimation(DependencyObject target)
        {
            var storyboard = new Storyboard();

            RepositionThemeAnimation repositionAnimation = new RepositionThemeAnimation
            {
                FromVerticalOffset = -50
            };
            Storyboard.SetTarget(repositionAnimation, target);

            var opacityAnimation = new DoubleAnimationUsingKeyFrames();
            var opacityStartFrame = new EasingDoubleKeyFrame();
            opacityStartFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
            opacityStartFrame.Value = 0;

            var opacityEndFrame = new EasingDoubleKeyFrame();
            opacityEndFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(250));
            opacityEndFrame.Value = 1;

            opacityAnimation.KeyFrames.Add(opacityStartFrame);
            opacityAnimation.KeyFrames.Add(opacityEndFrame);

            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            Storyboard.SetTarget(opacityAnimation, target);            

            storyboard.Children.Add(repositionAnimation);
            storyboard.Children.Add(opacityAnimation);
            return storyboard;
        }
    }
}
