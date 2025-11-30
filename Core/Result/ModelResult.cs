namespace SubmoduleTracker.Core.Result;
public sealed class ModelResult<T>
{
    public ResultCode ResultCode { get; set; }

    public T? Model { get; set; }

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