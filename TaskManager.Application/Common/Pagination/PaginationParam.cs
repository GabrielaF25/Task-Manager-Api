namespace TaskManager.Application.Common.Pagination;

public class PaginationParam
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; }
}
