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

        private object lockObject = new object();
        public new void Add(Place newPlace)
        {
            lock(lockObject)
            {
                base.Add(newPlace);
            }
        }

        public void AddSorted(Place newPlace)
        {
            lock(lockObject)
            {
                ExtensionMethods.ObservableCollectionExtensions.AddSorted(this, newPlace);
            }
        }

        public new void Remove(Place newPlace)
        {
            lock(lockObject)
            {
                base.Remove(newPlace);
            }
        }
    }
}
