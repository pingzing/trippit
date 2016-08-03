using System.Collections.Generic;
using System.Collections.ObjectModel;
using DigiTransit10.Models;

namespace DigiTransit10.Controls
{
    public class GroupedFavoriteList : ObservableCollection<IFavorite>
    {
        public string Key { get; set; }

        public GroupedFavoriteList(string header)
        {
            Key = header;
        }
    }
}