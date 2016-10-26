using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DigiTransit10.Models
{
    public class SecondaryTilePayload
    {
        public TileType SecondaryTileType { get; set; }
        public SimpleFavoritePlace[] SecondaryTilePlaces { get; set; }

        public SecondaryTilePayload() { } //Just for JSON.NET.

        private SecondaryTilePayload(TileType type, IEnumerable<SimpleFavoritePlace> places)
        {
            SecondaryTileType = type;
            SecondaryTilePlaces = places.ToArray();
        }

        public static SecondaryTilePayload Create(TileType type, IEnumerable<SimpleFavoritePlace> places)
        {
            return new SecondaryTilePayload(type, places);
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TileType
    {
        //Short member names to save on space.
        [EnumMember(Value = "fp")]
        FavoritePlace,

        [EnumMember(Value = "fr")]
        FavoriteRoute
    }
}
