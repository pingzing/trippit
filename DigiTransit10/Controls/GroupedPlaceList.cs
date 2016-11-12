using DigiTransit10.Models;
using System.Collections.ObjectModel;

namespace DigiTransit10.Controls
{
    public class GroupedPlaceList : ObservableCollection<IPlace>
    {
        public string Key { get; }
        public ModelEnums.PlaceType GroupType { get; }

        public GroupedPlaceList(ModelEnums.PlaceType type, string header)
        {
            GroupType = type;
            Key = header;
        }
    }
}
