namespace SubmoduleTracker.Core.Result;
public sealed class OperationResult
{
    public ResultCode ResultCode { get; set; }

    public string ErrorMessage { get; set; }

    private OperationResult() {}

    public static OperationResult WithSuccess()
    {
        return new OperationResult
        {
            ResultCode = ResultCode.Success,
            ErrorMessage = string.Empty
        };
    }

    public static OperationResult WithFailure(string errorMessage)
    {
        return new OperationResult
        {
            ResultCode = ResultCode.Failed,
            ErrorMessage = errorMessage
        };
    }
}