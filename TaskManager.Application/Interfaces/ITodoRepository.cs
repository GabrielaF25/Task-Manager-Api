using Task_Manager_Api.Models;
using TaskManager.Application.Pagination;
using TaskManager.Domain.Pagination;

namespace TaskManager.Application.Abstraction;

public interface ITodoRepository
{
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct);
    void Remove(TodoItem item);
    Task<IEnumerable<TodoItem>> GetAllAsync(QueryParamTodo queryParam,PaginationParam pagination,CancellationToken ct);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}