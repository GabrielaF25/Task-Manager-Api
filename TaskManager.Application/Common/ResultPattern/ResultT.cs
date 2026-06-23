namespace TaskManager.Application.Common.ResultPattern;

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T? data = default) => new()
    { 
        Data = data, 
        IsSuccess = true,
        StatusType = StatusType.Success
    };

    public static new Result<T> Failed(List<string> errors, StatusType statusType) => new()
    {
        Errors = errors,
        StatusType = statusType
    };
}
