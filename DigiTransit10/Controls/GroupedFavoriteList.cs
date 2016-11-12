using DigiTransit10.Models;
using System.Collections.ObjectModel;

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