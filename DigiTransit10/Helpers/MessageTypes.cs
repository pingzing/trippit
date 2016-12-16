using DigiTransit10.Models;
using Windows.Devices.Geolocation;

namespace DigiTransit10.Helpers
{
    /// <summary>
    /// Classes used as containers for use with MVVM Light's Messenger service.
    /// </summary>
    public static class MessageTypes
    {
        /// <summary>
        /// Indicates that a TripPlan has been found. The plan itself is stored in the SuspensionState dict.
        /// </summary>
        public class PlanFoundMessage
        {
            public VisualStateType VisualState { get; }

            public PlanFoundMessage(VisualStateType visualState)
            {
                VisualState = visualState;
            }
        }

        public enum VisualStateType
        {
            Narrow,
            Normal,
            Wide
        }

        /// <summary>
        /// Indicates that the active map should center on its collection of displayed favorites.
        /// </summary>
        public class CenterAroundFavoritesOnMap { }

        /// <summary>
        /// Indicates that the active map should center on the given location.
        /// </summary>
        public class CenterMapOnGeoposition
        {
            public BasicGeoposition Position { get; private set; }

            public CenterMapOnGeoposition(BasicGeoposition position)
            {
                Position = position;
            }
        }

        /// <summary>
        /// Indicates that a TripPlanStrip has been tapped, and we should zoom in to display its details.
        /// </summary>
        public class ViewPlanDetails
        {
            public TripItinerary BackingModel { get; private set; }

            public ViewPlanDetails(TripItinerary model)
            {
                BackingModel = model;
            }
        }

        /// <summary>
        /// Indicates that we should back out of a detailed TripPlanStrip view.
        /// </summary>
        public class ViewPlanStrips { }

        /// <summary>
        /// Generic indicator that navigation has recently been canceled.
        /// </summary>
        public class NavigationCanceled { }

        /// <summary>
        /// Message to any listeners that the caller would like to view the details of the given stop.
        /// </summary>
        public class ViewStopDetails
        {
            public TransitStop StopSelected { get; private set; }

            public ViewStopDetails(TransitStop stop)
            {
                StopSelected = stop;
            }
        }
    }
}
