namespace SubmoduleTracker.Core.Result;
public sealed class VoidResult
{
    public ResultCode ResultCode { get; set; }

    public string? ErrorMessage { get; set; }

    private VoidResult() {}

    public static VoidResult WithSuccess()
    {
        return new VoidResult
        {
            ResultCode = ResultCode.Success,
            ErrorMessage = string.Empty
        };
    }

    public static VoidResult WithFailure(string errorMessage)
    {
        return new VoidResult
        {
            ResultCode = ResultCode.Failed,
            ErrorMessage = errorMessage
        };
    }
}