using static DigiTransit10.Helpers.Enums;

namespace DigiTransit10.Helpers
{
    public interface IFailure
    {
        bool IsNone { get; }
        string FriendlyError { get; }
        IFailure None { get; }
        FailureReason Reason { get; }
    }
}