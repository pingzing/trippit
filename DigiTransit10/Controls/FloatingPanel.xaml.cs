using GalaSoft.MvvmLight.Command;
using System;
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
        // Divided into static and non-static so we can use it as both a Dependency Property's default
        // value AND reference it in XAML.
        public static double DefaultCollapsedPanelHeightStatic = 18;
        public double DefaultCollapsedPanelHeight => DefaultCollapsedPanelHeightStatic;

        //todo: rework into a DependencyProperty that actually opens/closes the panel
        public bool IsOpen => _currentState == _expandedState;

        private const int ExpandedPanelStateIndex = 0;
        private const int CollapsedPanelStateIndex = 1;

        private VisualState _currentState;
        private VisualState _expandedState;
        private VisualState _collapsedState;        

        public static readonly DependencyProperty CollapsedHeightProperty =
                    DependencyProperty.Register("CollapsedHeight", typeof(double), typeof(FloatingPanel), new PropertyMetadata(DefaultCollapsedPanelHeightStatic,
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
                CollapsingTranslationAnimation.To = translationDelta;
                VisualStateManager.GoToState(this, _collapsedState.Name, true);
            }
            else
            {
                double translationDelta = ExpandedHeight - CollapsedHeight;
                PanelExpandHeightValue.Value = ExpandedHeight;
                ExpandingTranslationAnimation.From = translationDelta;
                VisualStateManager.GoToState(this, _expandedState.Name, true);
            }
        }        
             
        private void GridGrabHeader_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Manip started!");
            // We crank up the Height here so that when the user drags the panel up, it's not visibly tiny and short.
            // The RenderTransform to is adjust back downward, and compensate for its sudden growth spurt.
            if(_currentState == _collapsedState)
            {
                PanelGrid.Height = ExpandedHeight;
                ((CompositeTransform)PanelGrid.RenderTransform).TranslateY = ExpandedHeight - CollapsedHeight;
            }
        }

        //todo: replace this series of manipulations with Composition namespace stuff. Perf on mobile SUCKS        
        private void GridGrabHeader_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {                       
            double proposedNewTrans = ((CompositeTransform)PanelGrid.RenderTransform).TranslateY + e.Delta.Translation.Y;            

            if (PanelGrid.Height - proposedNewTrans >= ExpandedHeight)
            {
                return;
            }
            if (PanelGrid.Height - proposedNewTrans <= GridGrabHeader.ActualHeight)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("Transforming panel to: " + proposedNewTrans);

            ((CompositeTransform)PanelGrid.RenderTransform).TranslateY = proposedNewTrans;
        }

        private void GridGrabHeader_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {                       
            double snapPoint = (ExpandedHeight - CollapsedHeight) * .3; //found by trial and error. "feels" right.
            double currentTransform = ((CompositeTransform)PanelGrid.RenderTransform).TranslateY;            

            if (_currentState == _expandedState)
            {
                if(currentTransform > snapPoint) 
                {
                    SnapCollapsedAfterManipulate();
                }
                else 
                {
                    SnapExpandedAfterManipulate(currentTransform);
                }
            }
            else //current state is collapsedState
            {
                if(ExpandedHeight - CollapsedHeight - currentTransform > snapPoint) 
                {
                    SnapExpandedAfterManipulate(currentTransform);
                }
                else
                {
                    SnapCollapsedAfterManipulate();
                }
            }          
        }

        private void SnapCollapsedAfterManipulate()
        {
            CollapsingTranslationAnimation.To = ExpandedHeight - CollapsedHeight;
            ExpandedToCollapsedStoryboard.Completed += CollapsedAfterManipulation_Completed;
            ExpandedToCollapsedStoryboard.Begin();
        }

        private void SnapExpandedAfterManipulate(double endTransform)
        {
            PanelExpandHeightValue.Value = ExpandedHeight;
            ExpandingTranslationAnimation.From = endTransform;
            CollapsedToExpandedStoryboard.Completed += ExpandedAfterManipulation_Completed;
            CollapsedToExpandedStoryboard.Begin();
        }

        private void CollapsedAfterManipulation_Completed(object sender, object e)
        {
            ExpandedToCollapsedStoryboard.Completed -= CollapsedAfterManipulation_Completed;
            if(_currentState != _collapsedState)
            {
                VisualStateManager.GoToState(this, _collapsedState.Name, false);
            }
            else
            {
                PanelGrid.Height = CollapsedHeight;                
            }
            ((CompositeTransform)PanelGrid.RenderTransform).TranslateY = 0;
        }

        private void ExpandedAfterManipulation_Completed(object sender, object e)
        {
            CollapsedToExpandedStoryboard.Completed -= ExpandedAfterManipulation_Completed;
            if(_currentState != _expandedState)
            {
                VisualStateManager.GoToState(this, _expandedState.Name, false);
            }
            else
            {
                PanelGrid.Height = ExpandedHeight;                
            }
            ((CompositeTransform)PanelGrid.RenderTransform).TranslateY = 0;
        }
    }
}
