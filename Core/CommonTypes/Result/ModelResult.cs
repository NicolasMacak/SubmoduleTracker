namespace SubmoduleTracker.Core.CommonTypes.Result;
/// <summary>
/// Container that holds information about execution of the operation
/// </summary>
/// <typeparam name="T">Type of data that operation may return</typeparam>
public sealed class ModelResult<T>
{
    /// <summary>
    /// Final state of the executed operation
    /// </summary>
    public ResultCode ResultCode { get; set; }
    /// <summary>
    /// Data returned by operation
    /// </summary>
    public T? Model { get; set; }
    /// <summary>
    /// Information about error when execution fails
    /// </summary>
    public string? ErrorMessage { get; set; }

    public ModelResult() { }

    public ModelResult<T> With(ResultCode resultCode, T? model = default)
    {
        ResultCode = resultCode;
        Model = model;

        return this;
    }

    public ModelResult<T> WithSuccess(T model)
    {
        ResultCode = ResultCode.Success;
        ErrorMessage = string.Empty;
        Model = model;
        
        return this;
    }

    public ModelResult<T> WithFailure(string errorMessage)
    {
        ResultCode = ResultCode.Failure;
        ErrorMessage = errorMessage;
        
        return this;
    }

}