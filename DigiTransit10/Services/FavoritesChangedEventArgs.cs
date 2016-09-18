using DigiTransit10.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DigiTransit10.Services
{
    public class FavoritesChangedEventArgs
    {
        public ReadOnlyCollection<IFavorite> AddedFavorites { get; }
        public ReadOnlyCollection<IFavorite> EditedFavorites { get; }
        public ReadOnlyCollection<IFavorite> RemovedFavorites { get; }

        public FavoritesChangedEventArgs(IList<IFavorite> added, IList<IFavorite> removed, IList<IFavorite> edited)
        {
            if (added != null) AddedFavorites = new ReadOnlyCollection<IFavorite>(added);
            if (removed != null) RemovedFavorites = new ReadOnlyCollection<IFavorite>(removed);
            if (edited != null) EditedFavorites = new ReadOnlyCollection<IFavorite>(edited);
        }
    }
}