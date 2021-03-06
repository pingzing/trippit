﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Trippit.Storyboards
{
    public sealed partial class ContinuumNavigationEntranceFactory : ResourceDictionary
    {
        private ContinuumNavigationEntranceFactory(FrameworkElement mover)
        {
            if (mover == null)
            {
                return;
            }

            this.InitializeComponent();

            mover.RenderTransform = new CompositeTransform();
            mover.Projection = new PlaneProjection();

            Storyboard.SetTarget(this.ScaleXComponent, mover);
            Storyboard.SetTarget(this.ScaleYComponent, mover);
            Storyboard.SetTarget(this.ProjectXComponent, mover);
            Storyboard.SetTarget(this.TranslateYComponent, mover);
            Storyboard.SetTarget(this.OpacityComponent, mover);
        }

        public static Storyboard GetAnimation(FrameworkElement mover)
        {
            return new ContinuumNavigationEntranceFactory(mover).ContinuumNavigationEntranceStoryboard;
        }
    }
}
