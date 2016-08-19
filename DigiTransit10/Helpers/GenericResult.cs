using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigiTransit10.Helpers.Enums;

namespace DigiTransit10.Helpers
{
    public struct GenericResult<TValue> : IResult<TValue>
    {
        public TValue Result { get; }
        public IFailure Failure { get; }

        public bool HasResult { get; }
        public bool IsFailure { get; }

        public GenericResult(TValue successVal)
        {
            Result = successVal;
            Failure = GenericFailure.GenericFailureNone;

            HasResult = true;
            IsFailure = false;
        }

        public GenericResult(GenericFailure failure)
        {
            Result = default(TValue);
            Failure = failure;

            HasResult = false;
            IsFailure = true;
        }

        public static GenericResult<TValue> Fail => new GenericResult<TValue>(new GenericFailure());

        public static GenericResult<TValue> FailWithReason(FailureReason reason)
        {
            return new GenericResult<TValue>(new GenericFailure(reason));
        }

        public static GenericResult<TValue> FailWithReason(string message, FailureReason reason)
        {
            return new GenericResult<TValue>(new GenericFailure(message, reason));
        }
    }

    public struct GenericFailure : IFailure
    {
        public bool IsNone { get; private set; }
        public FailureReason Reason { get; private set; }
        public string FriendlyError { get; private set; }
        public IFailure None => GenericFailure.GenericFailureNone;

        public static IFailure GenericFailureNone => new GenericFailure { IsNone = true, Reason = FailureReason.Unspecified, FriendlyError = null };

        public GenericFailure(FailureReason reason)
        {
            FriendlyError = "";
            Reason = reason;
            IsNone = false;
        }

        public GenericFailure(string message, FailureReason reason)
        {
            FriendlyError = message;
            Reason = reason;
            IsNone = false;
        }
    }   
}
