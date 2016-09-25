namespace DigiTransit10.Helpers
{
    public interface IResult
    {
        IFailure Failure { get; }
        bool IsSuccess { get; }
        bool IsFailure { get; }
    }

    public interface IResult<TValue>
    {
        TValue Result { get; }
        IFailure Failure { get; }

        bool HasResult { get; }
        bool IsFailure { get; }
    }
}