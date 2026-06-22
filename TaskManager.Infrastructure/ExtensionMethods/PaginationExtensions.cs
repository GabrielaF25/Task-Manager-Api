using TaskManager.Application.Common.Pagination;

namespace TaskManager.Infrastructure.ExtensionMethods;

public static class PaginationExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationParam pagination)
    {
        query = query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);

        return query;
    }
}
