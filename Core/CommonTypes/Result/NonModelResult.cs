namespace SubmoduleTracker.Core.CommonTypes.Result;
/// <summary>
/// Holds information about executed operation that does not return data
/// </summary>
public sealed class NonModelResult
{
    /// <summary>
    /// Final state of the executed operation
    /// </summary>
    public ResultCode ResultCode { get; set; }
    /// <summary>
    /// Data returned by operation
    /// </summary>
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