using System.Collections.ObjectModel;
using Trippit.Models;

namespace Trippit.Controls
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