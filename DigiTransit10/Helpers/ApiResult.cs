using static DigiTransit10.Helpers.Enums;

namespace DigiTransit10.Helpers
{
    public struct ApiResult<TValue> : IResult<TValue>
    {
        public TValue Result { get; }
        public IFailure Failure { get; }

        public bool HasResult { get; }
        public bool IsFailure { get; }

        public ApiResult(TValue successVal)
        {
            Result = successVal;
            Failure = ApiFailure.ApiFailureNone;

            HasResult = true;
            IsFailure = false;
        }

        public ApiResult(ApiFailure failure)
        {
            Result = default(TValue);
            Failure = failure;

            HasResult = false;
            IsFailure = true;
        }

        /// <summary>
        /// Return an <see cref="ApiResult{TValue}"/> with an Unspecified Failure.
        /// </summary>
        public static ApiResult<TValue> Fail => new ApiResult<TValue>(new ApiFailure());

        public static ApiResult<TValue> FailWithReason(FailureReason reason)
        {
            return new ApiResult<TValue>(new ApiFailure(reason));
        }

        public static ApiResult<TValue> FailWithMessage(string message, FailureReason reason)
        {
            return new ApiResult<TValue>(new ApiFailure(message, reason));
        }
    }

    public struct ApiFailure : IFailure
    {
        public bool IsNone { get; private set; }
        public FailureReason Reason { get; private set; }
        public string FriendlyError { get; private set; }
        public IFailure None => ApiFailure.ApiFailureNone;

        public static IFailure ApiFailureNone => new ApiFailure {IsNone = true, Reason = FailureReason.Unspecified, FriendlyError = null};

        public ApiFailure(FailureReason reason)
        {
            FriendlyError = "";
            Reason = reason;
            IsNone = false;
        }

        public ApiFailure(string message, FailureReason reason)
        {
            FriendlyError = message;
            Reason = reason;
            IsNone = false;
        }
    }
}