using Trippit.ViewModels;

namespace Trippit.Models
{
    public class SearchSectionChangedEventArgs
    {
        public SearchSection OldSection { get; }
        public SearchSection NewSection { get; }

        public SearchSectionChangedEventArgs(SearchSection oldSection, SearchSection newSection)
        {
            OldSection = oldSection;
            NewSection = newSection;
        }
    }
}
