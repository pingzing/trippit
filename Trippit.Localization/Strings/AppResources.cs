//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------
// <auto-generatedInfo>
// 	This code was generated by ResW File Code Generator (http://reswcodegen.codeplex.com)
// 	ResW File Code Generator was written by Christian Resma Helle
// 	and is under GNU General Public License version 2 (GPLv2)
// 
// 	This code contains a helper class exposing property representations
// 	of the string resources defined in the specified .ResW file
// 
// 	Generated: 08/17/2016 05:51:00
// </auto-generatedInfo>
// --------------------------------------------------------------------------------------------------
namespace DigiTransit10.Localization.Strings
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using Windows.ApplicationModel.Resources;


    public partial class AppResources
    {
        
        private static ResourceManager resourceLoader;
        private static CultureInfo ci;
        
        static AppResources()
        {
            string executingAssemblyName;
            executingAssemblyName = Windows.UI.Xaml.Application.Current.GetType().AssemblyQualifiedName;
            string[] executingAssemblySplit;
            executingAssemblySplit = executingAssemblyName.Split(',');
            executingAssemblyName = executingAssemblySplit[1];
            string currentAssemblyName;
            currentAssemblyName = typeof(AppResources).AssemblyQualifiedName;
            string[] currentAssemblySplit;
            currentAssemblySplit = currentAssemblyName.Split(',');
            currentAssemblyName = currentAssemblySplit[1];

            ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);

            resourceLoader = new ResourceManager(typeof(AppResources));
            
        }
        
        /// <summary>
        /// Localized resource similar to "You haven't saved any favorites yet"
        /// </summary>
        public static string Favorites_EmptyList
        {
            get
            {
                return resourceLoader.GetString("Favorites_EmptyList", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Favorites"
        /// </summary>
        public static string Favorites_FavoritesHeader
        {
            get
            {
                return resourceLoader.GetString("Favorites_FavoritesHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Date"
        /// </summary>
        public static string LiteralDate
        {
            get
            {
                return resourceLoader.GetString("LiteralDate", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "From"
        /// </summary>
        public static string MainPage_FromHeader
        {
            get
            {
                return resourceLoader.GetString("MainPage_FromHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "TRIP PLANNING"
        /// </summary>
        public static string MainPage_Header
        {
            get
            {
                return resourceLoader.GetString("MainPage_Header", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "To"
        /// </summary>
        public static string MainPage_ToHeader
        {
            get
            {
                return resourceLoader.GetString("MainPage_ToHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Addresses"
        /// </summary>
        public static string SuggestBoxHeader_Addresses
        {
            get
            {
                return resourceLoader.GetString("SuggestBoxHeader_Addresses", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Transit Stops"
        /// </summary>
        public static string SuggestBoxHeader_TransitStops
        {
            get
            {
                return resourceLoader.GetString("SuggestBoxHeader_TransitStops", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Arrival"
        /// </summary>
        public static string TimeType_Arrival
        {
            get
            {
                return resourceLoader.GetString("TimeType_Arrival", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Departure"
        /// </summary>
        public static string TimeType_Departure
        {
            get
            {
                return resourceLoader.GetString("TimeType_Departure", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Arrival or departure time?"
        /// </summary>
        public static string TimeType_PlaceholderText
        {
            get
            {
                return resourceLoader.GetString("TimeType_PlaceholderText", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Plan a trip"
        /// </summary>
        public static string TripForm_PlanATripHeader
        {
            get
            {
                return resourceLoader.GetString("TripForm_PlanATripHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Planning..."
        /// </summary>
        public static string TripForm_PlanningTrip
        {
            get
            {
                return resourceLoader.GetString("TripForm_PlanningTrip", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Plan"
        /// </summary>
        public static string TripForm_PlanTripButton
        {
            get
            {
                return resourceLoader.GetString("TripForm_PlanTripButton", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Resolving locations..."
        /// </summary>
        public static string TripFrom_GettingsAddresses
        {
            get
            {
                return resourceLoader.GetString("TripFrom_GettingsAddresses", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Fewer options"
        /// </summary>
        public static string TripForm_FewerOptions
        {
            get
            {
                return resourceLoader.GetString("TripForm_FewerOptions", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "More options"
        /// </summary>
        public static string TripForm_MoreOptions
        {
            get
            {
                return resourceLoader.GetString("TripForm_MoreOptions", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Time"
        /// </summary>
        public static string LiteralTime
        {
            get
            {
                return resourceLoader.GetString("LiteralTime", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Transit types"
        /// </summary>
        public static string TripForm_TransitTypeHeader
        {
            get
            {
                return resourceLoader.GetString("TripForm_TransitTypeHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "My location"
        /// </summary>
        public static string SuggestBoxHeader_MyLocation
        {
            get
            {
                return resourceLoader.GetString("SuggestBoxHeader_MyLocation", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Now"
        /// </summary>
        public static string LiteralNow
        {
            get
            {
                return resourceLoader.GetString("LiteralNow", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Set to today"
        /// </summary>
        public static string TripForm_SetDateToNow
        {
            get
            {
                return resourceLoader.GetString("TripForm_SetDateToNow", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Set to now"
        /// </summary>
        public static string TripForm_SetTimeToNow
        {
            get
            {
                return resourceLoader.GetString("TripForm_SetTimeToNow", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Origin"
        /// </summary>
        public static string TripPlanStrip_StartingPlaceDefault
        {
            get
            {
                return resourceLoader.GetString("TripPlanStrip_StartingPlaceDefault", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Loading..."
        /// </summary>
        public static string LoadingLiteral
        {
            get
            {
                return resourceLoader.GetString("LoadingLiteral", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Destination"
        /// </summary>
        public static string TripPlanStrip_EndPlaceDefault
        {
            get
            {
                return resourceLoader.GetString("TripPlanStrip_EndPlaceDefault", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Delete"
        /// </summary>
        public static string LiteralDelete
        {
            get
            {
                return resourceLoader.GetString("LiteralDelete", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Favorites"
        /// </summary>
        public static string SuggestBoxHeader_FavoritePlaces
        {
            get
            {
                return resourceLoader.GetString("SuggestBoxHeader_FavoritePlaces", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Now"
        /// </summary>
        public static string CurrentTimePicker_CurrentTime
        {
            get
            {
                return resourceLoader.GetString("CurrentTimePicker_CurrentTime", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Pinned favorites"
        /// </summary>
        public static string TripForm_PinnedFavoritesHeader
        {
            get
            {
                return resourceLoader.GetString("TripForm_PinnedFavoritesHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Trip plans"
        /// </summary>
        public static string TripResults_TripPlansHeader
        {
            get
            {
                return resourceLoader.GetString("TripResults_TripPlansHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "No routes found. Check your search parameters."
        /// </summary>
        public static string DialogMessage_NoTripsFoundNoResults
        {
            get
            {
                return resourceLoader.GetString("DialogMessage_NoTripsFoundNoResults", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "No routes found, because a connection with the server couldn't be established. Check your internet connection, or try again later."
        /// </summary>
        public static string DialogMessage_NoTripsFoundNoServer
        {
            get
            {
                return resourceLoader.GetString("DialogMessage_NoTripsFoundNoServer", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "No routes found. Please try again."
        /// </summary>
        public static string DialogMessage_NoTripsFoundUnknown
        {
            get
            {
                return resourceLoader.GetString("DialogMessage_NoTripsFoundUnknown", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "We were unable to locate the following places:"
        /// </summary>
        public static string DialogMessage_PlaceResolutionFailed
        {
            get
            {
                return resourceLoader.GetString("DialogMessage_PlaceResolutionFailed", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "We were unable to locate you. Are location services enabled for this app?"
        /// </summary>
        public static string DialogMessage_UserLocationFailed
        {
            get
            {
                return resourceLoader.GetString("DialogMessage_UserLocationFailed", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Location failure"
        /// </summary>
        public static string DialogTitle_NoLocationFound
        {
            get
            {
                return resourceLoader.GetString("DialogTitle_NoLocationFound", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "No routes found"
        /// </summary>
        public static string DialogTitle_NoTripsFound
        {
            get
            {
                return resourceLoader.GetString("DialogTitle_NoTripsFound", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Add favorite"
        /// </summary>
        public static string FavoritesPage_CommandBarAddFavorite
        {
            get
            {
                return resourceLoader.GetString("FavoritesPage_CommandBarAddFavorite", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "FAVORITES"
        /// </summary>
        public static string FavoritesPage_PageHeader
        {
            get
            {
                return resourceLoader.GetString("FavoritesPage_PageHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Favorites"
        /// </summary>
        public static string NavigationLabels_Favorites
        {
            get
            {
                return resourceLoader.GetString("NavigationLabels_Favorites", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Home"
        /// </summary>
        public static string NavigationLabels_Home
        {
            get
            {
                return resourceLoader.GetString("NavigationLabels_Home", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Search"
        /// </summary>
        public static string NavigationLabels_Search
        {
            get
            {
                return resourceLoader.GetString("NavigationLabels_Search", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Settings"
        /// </summary>
        public static string NavigationLabels_Settings
        {
            get
            {
                return resourceLoader.GetString("NavigationLabels_Settings", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Places"
        /// </summary>
        public static string Favorites_PlacesGroupHeader
        {
            get
            {
                return resourceLoader.GetString("Favorites_PlacesGroupHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Routes"
        /// </summary>
        public static string Favorites_RoutesGroupHeader
        {
            get
            {
                return resourceLoader.GetString("Favorites_RoutesGroupHeader", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Details"
        /// </summary>
        public static string TripResultCommandBar_Details
        {
            get
            {
                return resourceLoader.GetString("TripResultCommandBar_Details", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Map"
        /// </summary>
        public static string TripResultCommandBar_Map
        {
            get
            {
                return resourceLoader.GetString("TripResultCommandBar_Map", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Unpin"
        /// </summary>
        public static string PinnedFavoritesList_Unpin
        {
            get
            {
                return resourceLoader.GetString("PinnedFavoritesList_Unpin", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "intermediate stop(s)"
        /// </summary>
        public static string TripDetailListIntermediates_IntermediateStopsNumber
        {
            get
            {
                return resourceLoader.GetString("TripDetailListIntermediates_IntermediateStopsNumber", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Walk {0} meters"
        /// </summary>
        public static string TripDetailListIntermediates_WalkDistance
        {
            get
            {
                return resourceLoader.GetString("TripDetailListIntermediates_WalkDistance", ci);
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Travel {0} meters"
        /// </summary>
        public static string TripDetailListIntermediates_TransitDistance
        {
            get
            {
                return resourceLoader.GetString("TripDetailListIntermediates_TransitDistance", ci);
            }
        }
    }
}
