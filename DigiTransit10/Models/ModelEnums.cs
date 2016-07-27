using System;

namespace DigiTransit10.Models
{
    public static class ModelEnums
    {
        public enum PlaceType
        {
            UserCurrentLocation,
            Stop,
            Address,
            Coordinates,
            NameOnly,
            FavoritePlace
        }

        [Flags]
        public enum ApiFailureReason
        {
            Unspecified = 0,
            NoConnection = 1,
            ServerDown = 2,
            InternalServerError = 4,
            InvalidCredentials = 8,
            NoResults = 16,
        }
    }
}
