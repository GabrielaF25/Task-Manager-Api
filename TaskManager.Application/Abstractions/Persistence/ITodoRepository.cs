using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Todos.Queries.GetTodos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Persistence;

public interface ITodoRepository
{
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct);
    void Remove(TodoItem item);
    Task<PaginationResult<TodoItem>> GetAllAsync(QueryParamTodo queryParam,PaginationParam pagination,CancellationToken ct);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}