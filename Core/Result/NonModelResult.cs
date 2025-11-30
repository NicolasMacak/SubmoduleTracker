namespace SubmoduleTracker.Core.Result;
public sealed class NonModelResult
{
    public ResultCode ResultCode { get; set; }

    public string? ErrorMessage { get; set; }

    private NonModelResult() {}

    public static NonModelResult WithSuccess()
    {
        return new NonModelResult
        {
            ResultCode = ResultCode.Success,
            ErrorMessage = string.Empty
        };
    }

    public static NonModelResult WithFailure(string errorMessage)
    {
        return new NonModelResult
        {
            ResultCode = ResultCode.Failure,
            ErrorMessage = errorMessage
        };
    }
}