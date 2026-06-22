namespace TaskManager.Application.Common.ResultPattern;


public class Result
{
    public StatusType StatusType { get; set; }
    public bool IsSuccess { get; set; } = false;
    public List<string> Errors { get; set; } = new List<string>();

    public static Result Success() => new()
    {
        IsSuccess = true,
        StatusType = StatusType.Success
    };

    public static Result Failed(List<string> errors, StatusType statusType) => new()
    {
        Errors = errors,
        StatusType = statusType
    };
}

