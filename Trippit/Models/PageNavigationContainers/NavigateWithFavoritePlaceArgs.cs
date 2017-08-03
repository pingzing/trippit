using Trippit.Models;

namespace Trippit.Helpers.PageNavigationContainers
{
    public class NavigateWithFavoritePlaceArgs
    {
        public IPlace Place { get; set; }
        public NavigationType PlaceNavigationType { get; set; }
        public int? IntermediateIndex { get; set; }

        public NavigateWithFavoritePlaceArgs(IPlace navigationPlace, NavigationType navigationType, int? intermediateIndex = null)
        {
            Place = navigationPlace;
            PlaceNavigationType = navigationType;
            IntermediateIndex = intermediateIndex;
        }
    }

    public enum NavigationType
    {
        AsOrigin,
        AsDestination,
        AsIntermediate
    }
}
