namespace DigiTransit10.Helpers
{
    public interface IResult<TValue>
    {
        TValue Result { get; }
        IFailure Failure { get; }

        bool HasResult { get; }
        bool IsFailure { get; }            
    }
}