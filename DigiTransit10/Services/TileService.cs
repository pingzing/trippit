using DigiTransit10.Models;
using DigiTransit10.Services.SettingsServices;
using MetroLog;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.StartScreen;

namespace DigiTransit10.Services
{
    public interface ITileService
    {
        Task PinFavoriteToStartAsync(IFavorite favorite);
    }

    public class TileService : ITileService
    {
        private readonly SettingsService _settingsService;
        private readonly ILogger _logger;
        public TileService(SettingsService settings, ILogger logger)
        {
            _settingsService = settings;
            _logger = logger;
        }

        public async Task PinFavoriteToStartAsync(IFavorite favorite)
        {
            string tileArgs = GetTileArgs(favorite);
            Uri imageUri = new Uri("ms-appx:///Assets/Images/Square150x150Logo.png");
            var tile = new SecondaryTile(favorite.FavoriteId.ToString(), favorite.UserChosenName, tileArgs, imageUri, TileSize.Square150x150);
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Images/Wide310x150Logo.png");
            tile.VisualElements.Square71x71Logo = new Uri("ms-appx:///Assets/Images/Square71x71Logo.png");
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.ShowNameOnWide310x150Logo = true;
            tile.VisualElements.BackgroundColor = Colors.Transparent;
            tile.RoamingEnabled = true;
            bool pinned = await tile.RequestCreateAsync();
        }

        private string GetTileArgs(IFavorite favorite)
        {
            var place = favorite as FavoritePlace;
            if (place != null)
            {
                var tilePayload = SecondaryTilePayload.Create(TileType.FavoritePlace, new SimpleFavoritePlace[]
                {
                    new SimpleFavoritePlace { Lat = place.Lat, Lon = place.Lon, Name = place.UserChosenName }
                });
                return JsonConvert.SerializeObject(tilePayload, Formatting.None);
            }

            var route = favorite as FavoriteRoute;
            if (route != null)
            {
                var tilePayload = SecondaryTilePayload.Create(TileType.FavoriteRoute, route.RoutePlaces);
                return JsonConvert.SerializeObject(tilePayload, Formatting.None);
            }
            return null;
        }
    }
}
