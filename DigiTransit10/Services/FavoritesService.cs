using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services.SettingsServices;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DigiTransit10.Services
{
    public interface IFavoritesService : IAsyncInitializable
    {
        void AddFavorite(IFavorite newFave);
        bool RemoveFavorite(IFavorite deletedFave);
        Task<IReadOnlyList<IFavorite>> GetFavoritesAsync();
        Task FlushFavoritesAsync();
        //void AddBookmarkedItinerary(TripItinerary newBookmark);
        //bool DeleteBookmarkedItinerary(TripItinerary deletedBookmark);
    }

    public class FavoritesService : IFavoritesService
    {
        private const string FavoritesFileNameV1 = "_favorites_v1.json";
        private readonly SettingsService _settingsService;
        private readonly IFileService _fileService;

        private IStorageFile _favoritesFile;

        public Task Initialization { get; private set; }

        public FavoritesService(SettingsService settingsService, IFileService fileService)
        {
            _settingsService = settingsService;
            _fileService = fileService;
            Initialization = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _favoritesFile = await _fileService.GetStorageFileAsync(FavoritesFileNameV1);
        }

        /// <summary>
        /// Access a read-only collection of the user's IFavorites. To Add or Remove, call the AddFavorite() or RemoveFavorite() methods.
        /// </summary>
        private List<IFavorite> _favorites;
        public async Task<IReadOnlyList<IFavorite>> GetFavoritesAsync()
        {
            if (_favorites == null)
            {
                await Initialization; //make sure we've got the file ready
                List<IFavorite> favorite = await DeserializeFavoritesFile();
                if (favorite == null)
                {
                    _favorites = new List<IFavorite>();
                    return _favorites.AsReadOnly();
                }
                else
                {
                    _favorites = favorite;
                    return _favorites.AsReadOnly();
                }
            }
            else
            {
                return _favorites;
            }
        }

        /// <summary>
        /// Add favorite to backing list behind <see cref="Favorites"/>, and flushes the changed favorites list to storage.
        /// </summary>
        /// <param name="newFavorite"></param>
        public void AddFavorite(IFavorite newFavorite)
        {
            _favorites.Add(newFavorite);
            Messenger.Default.Send(new MessageTypes.FavoritesChangedMessage(new List<IFavorite> { newFavorite }, null));
        }

        public bool RemoveFavorite(IFavorite toRemove)
        {
            bool success = _favorites.Remove(toRemove);
            if (success)
            {
                Messenger.Default.Send(new MessageTypes.FavoritesChangedMessage(null, new List<IFavorite> { toRemove }));
            }
            return success;
        }

        public async Task FlushFavoritesAsync()
        {
            await Initialization; //make sure we've got the file ready

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            };

            try
            {           
                using (Stream outStream = await _favoritesFile.OpenStreamForWriteAsync())
                using (var gzip = new GZipStream(outStream, CompressionLevel.Optimal))
                {                
                    gzip.SerializeJsonToStream(_favorites, settings);
                }
            }
            catch(IOException ex)
            {
                //todo: Log me. Should we inform the user? There's nothing they can do, but info might be handy.
                System.Diagnostics.Debug.WriteLine($"Could not serialize favorites: {ex}: {ex.Message}");
            }
        }

        private async Task<List<IFavorite>> DeserializeFavoritesFile()
        {
            await Initialization;

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };

            try
            {
                using (Stream inStream = await _favoritesFile.OpenStreamForReadAsync())
                using (var gzip = new GZipStream(inStream, CompressionMode.Decompress))
                {
                    return gzip.DeseriaizeJsonFromStream<List<IFavorite>>(settings);
                }
            }
            catch(IOException ex)
            {
                //todo: Log me
                System.Diagnostics.Debug.WriteLine($"Could not deserialize favorites: {ex}: {ex.Message}");
                return null;
            }
        }
    }
}
