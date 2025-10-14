namespace SubmoduleTracker.Core.Result;
public sealed class ModelResult<T>
{
    public ResultCode ResultCode { get; set; }

    public T? Model { get; set; }

    public string? ErrorMessage { get; set; }

    private ModelResult() { }

    public static ModelResult<T> WithSuccess()
    {
        return new ModelResult<T>
        {
            ResultCode = ResultCode.Success,
            ErrorMessage = string.Empty
        };
    }

    public static ModelResult<T> WithFailure(string errorMessage)
    {
        return new ModelResult<T> 
        {
            ResultCode = ResultCode.Failed,
            ErrorMessage = errorMessage
        };
    }

}