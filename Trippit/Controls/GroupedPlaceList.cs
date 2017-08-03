using System.Collections.ObjectModel;
using Trippit.Models;

namespace Trippit.Controls
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
