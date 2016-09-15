using DigiTransit10.Models;

namespace DigiTransit10.Helpers
{
    public static class MessageTypes
    {
        public class PlanFoundMessage
        {
            public VisualStateType VisualState { get; }

            public PlanFoundMessage(VisualStateType visualState)
            {
                VisualState = visualState;
            }
        }

        public class CenterAroundFavoritesOnMap { }        

        public class ViewPlanDetails
        {
            public TripItinerary BackingModel { get; private set; }            

            public ViewPlanDetails(TripItinerary model)
            {
                BackingModel = model;                
            }
        }

        public enum VisualStateType
        {
            Narrow,
            Normal,
            Wide
        }

        public class ViewPlanStrips { }

        public class NavigationCanceled { }
    }
}
