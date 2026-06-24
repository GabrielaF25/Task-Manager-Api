using TaskManager.Application.Features.Projects.Queries.GetProjects;
using TaskManager.Application.Features.Todos.Queries.GetTodos;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.ExtensionMethods;

public static class QueryFeaturesExtension
{
    public static IQueryable<TodoItem> ApplyTodoFilters(this IQueryable<TodoItem> query, QueryParamTodo filter)
    {
        if(!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(t =>t.Title.ToLower().Contains(search));
        }

        if(filter.IsCompleted is not null)
        {
            var isCompleted = filter.IsCompleted;
            query = query.Where(t => t.IsCompleted == isCompleted);
        }

      
        return query;
    }

    public static IQueryable<TodoItem> ApplyTodoSorting(this IQueryable<TodoItem> query, QueryParamTodo sortingBy)
    {
        if (!string.IsNullOrWhiteSpace(sortingBy.SortBy))
        {
            var sortBy = sortingBy.SortBy.Trim().ToLower();
            var isDescending = sortingBy.SortDirection?
                .Equals("desc", StringComparison.InvariantCultureIgnoreCase) == true;
            query = sortBy switch
            {
                "title" => isDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "id" => isDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
                "date" => isDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                _ => query
            };

        }
        else
        {
            query = query.OrderBy(t => t.Id);
        }
            return query;
    }

    public static IQueryable<Project> ApplyProjectFilters
        (this  IQueryable<Project> query, QueryParamProject filter)
    {
        var search = filter.Search?.Trim().ToLower();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.ToLower().Contains(search));
        }


        return query;
    }

    public static IQueryable<Project> ApplyProjectSorting
     (this IQueryable<Project> query, QueryParamProject filter)
    {
        var sortBy = filter.SortBy?.Trim().ToLower();

        var isDescending = filter.SortDirection?
            .Equals("desc", StringComparison.InvariantCultureIgnoreCase) == true;

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy switch
            {
                "id" => isDescending? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                _ => query
            };
        }
        else
        {
            query = query.OrderBy(p => p.Id);
        }

        return query;
    }
}