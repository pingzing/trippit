using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigiTransit10.Models;

namespace DigiTransit10.Controls
{
    public class GroupedPlaceList : ObservableCollection<Place>
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
