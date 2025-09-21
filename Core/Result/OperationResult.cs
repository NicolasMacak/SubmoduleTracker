namespace SubmoduleTracker.Core.Result;
public sealed class OperationResult
{
    public ResultCode ResultCode { get; set; }

    public string ErrorMessage { get; set; } 

    public OperationResult()
    {
        ResultCode = ResultCode.Success;
        ErrorMessage = string.Empty;
    }

    public OperationResult(string errorMessage)
    {
        ResultCode = ResultCode.Failed;
        ErrorMessage = errorMessage;
    }

}