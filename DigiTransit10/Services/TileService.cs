using DigiTransit10.Models;
using DigiTransit10.Services.SettingsServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.StartScreen;

namespace DigiTransit10.Services
{
    public interface ITileService
    {
        Task PinFavoritePlaceToStartAsync(FavoritePlace favorite);
    }

    public class TileService : ITileService
    {
        private readonly SettingsService _settingsService;
        public TileService(SettingsService settings)
        {
            _settingsService = settings;
        }

        public async Task PinFavoritePlaceToStartAsync(FavoritePlace favorite)
        {
            Guid tileId = Guid.NewGuid();
            string tileArgs = $"?userChosenName={favorite.UserChosenName}&lat={favorite.Lat.ToString(CultureInfo.InvariantCulture)}&lon={favorite.Lon.ToString(CultureInfo.InvariantCulture)}";
            Uri imageUri = new Uri("ms-appx:///Assets/Images/Square150x150Logo.png");
            SecondaryTile tile = new SecondaryTile(tileId, favorite.UserChosenName, tileArgs, imageUri, TileSize.Square150x150);
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Images/Wide310x150Logo.png");
            tile.VisualElements.Square71x71Logo = new Uri("ms-appx:///Assets/Images/Square71x71Logo.png");
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.ShowNameOnWide310x150Logo = true;
            tile.VisualElements.BackgroundColor = Colors.Transparent;
            tile.RoamingEnabled = true;
            bool pinned = await tile.RequestCreateAsync();
            if(pinned)
            {
                _settingsService.SecondaryTileIds.Add(tileId);
            }
        }
    }
}
