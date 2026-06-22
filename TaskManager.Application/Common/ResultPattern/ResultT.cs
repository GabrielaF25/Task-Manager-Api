namespace TaskManager.Application.Common.ResultPattern;


public class Result<T>
{
    public StatusType StatusType { get; set; }
    public bool IsSuccess { get; set;} = false;
    public List<string> Errors { get; set; } = new List<string>();
    public T? Data { get; set; }

    public static Result<T> Success(T? data = default) => new()
    { 
        Data = data, 
        IsSuccess = true,
        StatusType = StatusType.Success
    };

    public static Result<T> Failed(List<string> errors, StatusType statusType) => new()
    {
        Errors = errors,
        StatusType = statusType
    };
}
