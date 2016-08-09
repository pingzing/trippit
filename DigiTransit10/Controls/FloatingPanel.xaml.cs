using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    [ContentProperty(Name = "InnerContent")]
    public sealed partial class FloatingPanel : UserControl
    {
        private const int ExpandedPanelStateIndex = 0;
        private const int CollapsedPanelStateIndex = 1;

        private VisualState _currentState;
        private VisualState _expandedState;
        private VisualState _collapsedState;

        public static readonly DependencyProperty CollapsedHeightProperty =
                    DependencyProperty.Register("CollapsedHeight", typeof(double), typeof(FloatingPanel), new PropertyMetadata(0.0,
                        OnCollapsedHeightChanged));
        private static void OnCollapsedHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as FloatingPanel;
            if(_this == null)
            {
                return;
            }

            double newCollapsed = (double)e.NewValue;
            _this.CollapsedHeightKeyFrame.Value = newCollapsed;
            if(_this._currentState == _this._collapsedState)
            {
                _this.PanelGrid.Height = newCollapsed;
            }
        }
        public double CollapsedHeight
        {
            get { return (double)GetValue(CollapsedHeightProperty); }
            set { SetValue(CollapsedHeightProperty, value); }
        }

        public static readonly DependencyProperty ExpandedHeightProperty =
            DependencyProperty.Register("ExpandedHeight", typeof(double), typeof(FloatingPanel), new PropertyMetadata(0.0,
                OnExpandedHeightChanged));
        private static void OnExpandedHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as FloatingPanel;
            if (_this == null)
            {
                return;
            }

            double newExpanded = (double)e.NewValue;
            _this.ExpandedHeightKeyFrame.Value = newExpanded;
            if(_this._currentState == null || _this._currentState == _this._expandedState)
            {
                _this.PanelGrid.Height = newExpanded;
            }
        }
        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(object), typeof(FloatingPanel), new PropertyMetadata(null));
        public object InnerContent
        {
            get { return (object)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }               

        public double ExpandedHeight
        {
            get { return (double)GetValue(ExpandedHeightProperty); }
            set { SetValue(ExpandedHeightProperty, value); }
        }

        public FloatingPanel()
        {
            this.InitializeComponent();            
            this.Loaded += FloatingPanel_Loaded;
            this.FloatingPanelStateGroup.CurrentStateChanged += (s, e) =>
            {
                _currentState = e.NewState;
            };
        }

        private void FloatingPanel_Loaded(object sender, RoutedEventArgs e)
        {            
            _expandedState = FloatingPanelStateGroup.States[ExpandedPanelStateIndex];
            _collapsedState = FloatingPanelStateGroup.States[CollapsedPanelStateIndex];

            ExpandedHeightKeyFrame.Value = ExpandedHeight;
            CollapsedHeightKeyFrame.Value = CollapsedHeight;
                        
            ExpandedHeightStoryboard.Completed += (s1, args1) =>
            {
                PanelGrid.Height = ExpandedHeight;
            };

            CollapsedHeightStoryboard.Completed += (s2, args2) =>
            {
                PanelGrid.Height = CollapsedHeight;
            };

            VisualStateManager.GoToState(this, _currentState?.Name ?? _expandedState.Name, false);
        }

        private void GridGrabHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_currentState == null ||
                _currentState == _expandedState)
            {
                double translationDelta = ExpandedHeight - CollapsedHeight;
                ExpandedTranslationAnimation.To = translationDelta;
                VisualStateManager.GoToState(this, _collapsedState.Name, true);
            }
            else
            {
                double translationDelta = ExpandedHeight - CollapsedHeight;
                PanelExpandHeightValue.Value = ExpandedHeight;
                PanelExpandTranslateStart.From = translationDelta;
                VisualStateManager.GoToState(this, _expandedState.Name, true);
            }
        }

        private Point _manipStartedPoint = new Point();
        private void GridGrabHeader_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _manipStartedPoint = e.Position;            
        }

        private void GridGrabHeader_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double proposedNewTrans = ((CompositeTransform)PanelGrid.RenderTransform).TranslateY + e.Delta.Translation.Y;

            if(PanelGrid.Height - proposedNewTrans >= ExpandedHeight)
            {
                return;
            }
            if (PanelGrid.Height - proposedNewTrans <= GridGrabHeader.ActualHeight)
            {
                return;
            }

            ((CompositeTransform)PanelGrid.RenderTransform).TranslateY = proposedNewTrans;
        }

        private void GridGrabHeader_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            //snap to collapsed or expanded based on where the user stopped dragging            
        }
    }
}
