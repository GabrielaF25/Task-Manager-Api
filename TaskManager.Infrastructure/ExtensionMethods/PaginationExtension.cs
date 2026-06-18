using System.Linq;
using Task_Manager_Api.Models;
using TaskManager.Application.Pagination;
using TaskManager.Domain.Pagination;

namespace TaskManager.Infrastructure.ExtensionMethods;

public static class PaginationExtension
{
    public static IQueryable<TodoItem> Filter(this IQueryable<TodoItem> query, QueryParamTodo pagination)
    {
        if(!string.IsNullOrWhiteSpace( pagination.Search))
        {
            var searby = pagination.Search.Trim().ToLower();
            query = query.Where(t =>t.Title.ToLower().Contains(searby));
        }

        if(pagination.IsCompleted is not null)
        {
            var isCompleted = pagination.IsCompleted;
            query = query.Where(t => t.IsCompleted == isCompleted);
        }

        if(!string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            var sortBy = pagination.SortBy.Trim().ToLower();
            var isDescending = pagination.SortDirection?
                .Equals("desc", StringComparison.InvariantCultureIgnoreCase) == true;
            query = (sortBy) switch
            {
                ("title") =>isDescending? query.OrderByDescending(t => t.Title) :query.OrderBy(t => t.Title),
                ("id") => isDescending ? query.OrderByDescending(t => t.Id): query.OrderBy(t => t.Id),
                ("date") => isDescending ? query.OrderByDescending(t =>t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                _ => query
            };

        }

        return query;
    }
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationParam pagination)
    {
        query = query
            .Skip((pagination.PageNumber-1)*pagination.PageSize)
            .Take(pagination.PageSize);

        return query;
    }
}