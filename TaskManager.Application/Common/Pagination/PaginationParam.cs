namespace TaskManager.Application.Common.Pagination;

public class PaginationParam
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value <= 0 || value > MaxPageSize ? MaxPageSize : value;               
        }
    }
}
