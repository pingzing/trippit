using static DigiTransit10.Models.ModelEnums;

namespace DigiTransit10.Helpers
{
    public struct ApiResult<TValue>
    {
        public TValue Result { get; }
        public ApiFailure Failure { get; }

        public bool HasResult { get; }
        public bool IsFailure { get; }

        public ApiResult(TValue successVal)
        {
            Result = successVal;
            Failure = ApiFailure.None;

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

        public static ApiResult<TValue> FailWithReason(ApiFailureReason reason)
        {
            return new ApiResult<TValue>(new ApiFailure(reason));
        }

        public static ApiResult<TValue> FailWithMessage(string message, ApiFailureReason reason)
        {
            return new ApiResult<TValue>(new ApiFailure(message, reason));
        }
    }

    public struct ApiFailure
    {
        public bool IsNone { get; private set; }
        public ApiFailureReason Reason { get; private set; }
        public string FriendlyError { get; private set; }

        public static ApiFailure None => new ApiFailure {IsNone = true, Reason = ApiFailureReason.Unspecified, FriendlyError = null};

        public ApiFailure(ApiFailureReason reason)
        {
            FriendlyError = "";
            Reason = reason;
            IsNone = false;
        }

        public ApiFailure(string message, ApiFailureReason reason)
        {
            FriendlyError = message;
            Reason = reason;
            IsNone = false;
        }
    }
}