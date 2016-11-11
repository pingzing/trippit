using DigiTransit10.ViewModels;

namespace DigiTransit10.Models
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
