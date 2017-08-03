using static Trippit.Helpers.Enums;

namespace Trippit.Helpers
{
    public interface IFailure
    {
        bool IsNone { get; }
        string FriendlyError { get; }
        IFailure None { get; }
        FailureReason Reason { get; }
    }
}