using System.Security.Principal;

namespace TaskManager.Application.Common.Pagination;

public static class PageResultExtension
{
    public static  PaginationResult<T> TransformToPageList<T>(this IEnumerable<T> data, PaginationParam pagination, int totalData)
    {
        var pageResult = new PaginationResult<T>()
        {
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalData,      
            Items = data
        };

        pageResult.TotalPages = (int)Math.Ceiling(totalData / (double)pagination.PageSize);

        pageResult.HasNextPage = pagination.PageNumber < pageResult.TotalPages;
        pageResult.HasPreviousPage  = pagination.PageNumber > 1;

        return pageResult;
    }
}

