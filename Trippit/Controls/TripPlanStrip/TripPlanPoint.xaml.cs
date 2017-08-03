using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trippit.Controls.TripPlanStrip
{
    public sealed partial class TripPlanPoint : UserControl
    {
        private const int IsEndOrMidColumn = 0;
        private const int IsStartColumn = 1;
        private const int IsStartOrEndColumnSpan = 1;
        private const int IsMiddleColumnSpan = 2;

        public static readonly DependencyProperty PlaceNameProperty =
            DependencyProperty.Register("PlaceName", typeof(string), typeof(TripPlanPoint), new PropertyMetadata(""));
        public string PlaceName
        {
            get { return (string)GetValue(PlaceNameProperty); }
            set { SetValue(PlaceNameProperty, value); }
        }

        public static readonly DependencyProperty PointTimeProperty =
            DependencyProperty.Register("PointTime", typeof(DateTime), typeof(TripPlanPoint), new PropertyMetadata(default(DateTime)));
        public DateTime PointTime
        {
            get { return (DateTime)GetValue(PointTimeProperty); }
            set { SetValue(PointTimeProperty, value); }
        }

        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(TripPlanPoint), new PropertyMetadata(false, new PropertyChangedCallback(IsStartChanged)));
        private static void IsStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripPlanPoint thisControl = d as TripPlanPoint;
            if(thisControl == null)
            {
                return;
            }
            if(e.NewValue is bool)
            {
                bool newIsStart = (bool)e.NewValue;
                if(newIsStart)
                {
                    if(thisControl.IsEnd)
                    {
                        throw new ArgumentException("IsStart and IsEnd cannot both be true");
                    }
                    thisControl.PlanStripRectangle.SetValue(Grid.ColumnProperty, IsStartColumn);
                    thisControl.PlanStripRectangle.SetValue(Grid.ColumnSpanProperty, IsStartOrEndColumnSpan);
                }
                else
                {
                    if (thisControl.IsEnd)
                    {

                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnProperty, IsEndOrMidColumn);
                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnSpanProperty, IsStartOrEndColumnSpan);
                    }
                    else
                    {
                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnProperty, IsEndOrMidColumn);
                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnSpanProperty, IsMiddleColumnSpan);
                    }
                }
            }
        }
        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }                

        public static readonly DependencyProperty IsEndProperty =
            DependencyProperty.Register("IsEnd", typeof(bool), typeof(TripPlanPoint), new PropertyMetadata(false, new PropertyChangedCallback(IsEndChanged)));
        private static void IsEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TripPlanPoint thisControl = d as TripPlanPoint;
            if(thisControl == null)
            {
                return;
            }
            if(e.NewValue is bool)
            {
                bool newIsend = (bool)e.NewValue;
                if(newIsend)
                {
                    if(thisControl.IsStart)
                    {
                        throw new ArgumentException("IsStart and IsEnd cannot both be true.");
                    }

                    thisControl.PlanStripRectangle.SetValue(Grid.ColumnProperty, IsEndOrMidColumn);
                    thisControl.PlanStripRectangle.SetValue(Grid.ColumnSpanProperty, IsStartOrEndColumnSpan);
                }
                else
                {                    
                    if(thisControl.IsStart)
                    {
                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnProperty, IsStartColumn);
                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnSpanProperty, IsStartOrEndColumnSpan);
                    }
                    else
                    {
                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnProperty, IsEndOrMidColumn);
                        thisControl.PlanStripRectangle.SetValue(Grid.ColumnSpanProperty, IsMiddleColumnSpan);
                    }
                }
            }
        }
        public bool IsEnd
        {
            get { return (bool)GetValue(IsEndProperty); }
            set { SetValue(IsEndProperty, value); }
        }                        

        public TripPlanPoint()
        {
            this.InitializeComponent();
        }       
    }
}
