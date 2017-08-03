using System;

namespace Trippit.Helpers
{
    public static class Enums
    {
        [Flags]
        public enum FailureReason
        {
            Unspecified = 0,
            NoConnection = 1,
            ServerDown = 2,
            InternalServerError = 4,
            InvalidCredentials = 8,
            NoResults = 16,
            Canceled = 32,
            Exists = 64,
        }
    }
}
