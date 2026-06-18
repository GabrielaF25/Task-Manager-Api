using TaskManager.Application.Features.Todo.Dtos;
using TaskManager.Application.Pagination;
using TaskManager.Domain.Pagination;

namespace TaskManager.Application.Interfaces
{
    public interface ITodoService
    {
        Task<TodoResponse> AddItemAsync(CreateTodoRequest todoRequest, CancellationToken ct);
        Task<bool> DeleteItemAsync(int id, CancellationToken ct);
        Task<IEnumerable<TodoResponse>> GetAllAsync(QueryParamTodo queryParam, PaginationParam pagination,CancellationToken ct);
        Task<TodoResponse?> GetByIdAsync(int id, CancellationToken ct);
        Task<TodoResponse?> UpdateItemStatusAsync(int id, CancellationToken ct);
    }
}