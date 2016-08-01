using DigiTransit10.Storyboards;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace DigiTransit10.Behaviors
{
    public class AnimateVisibilityBehavior : DependencyObject, IBehavior
    {
        long _callbackToken;
        Storyboard _animationStoryboard = null;

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            _callbackToken = AssociatedObject.RegisterPropertyChangedCallback(UIElement.VisibilityProperty, OnVisibilityChanged);
        }

        public void Detach()
        {
            AssociatedObject.UnregisterPropertyChangedCallback(UIElement.VisibilityProperty, _callbackToken);
        }

        private void OnVisibilityChanged(DependencyObject sender, DependencyProperty dp)
        {
            UIElement _this = sender as UIElement;
            if(_this == null)
            {
                return;
            }

            if ((Visibility)_this.GetValue(dp) == Visibility.Collapsed)
            {
                return;
            }

            if (_animationStoryboard == null)
            {
                _animationStoryboard = FadeInDownwardFactory.GetAnimation(_this);
            }            

            _animationStoryboard.Begin();            
        }
    }
}
