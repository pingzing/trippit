using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigiTransit10.Helpers.Enums;

namespace DigiTransit10.Models
{
    /// <summary>
    /// Contains either an IPlace from successfully resolving a place, or a FailureReason explaining why it failed, and the IPlace we were trying to resolve.
    /// </summary>
    public class PlaceResolutionResult
    {
        public bool IsFailure { get; private set; }
        public FailureReason Reason {get; private set;}
        public IPlace AttemptedResolvedPlace { get; private set; }

        public PlaceResolutionResult(IPlace place)
        {
            AttemptedResolvedPlace = place;
        }

        public PlaceResolutionResult(IPlace place, FailureReason reason)
        {
            IsFailure = true;
            Reason = reason;
            AttemptedResolvedPlace = place;
        }

        public PlaceResolutionResult(FailureReason reason)
        {
            IsFailure = true;
            Reason = reason;
        }
    }    
}
