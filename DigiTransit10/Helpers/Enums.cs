using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTransit10.Helpers
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
