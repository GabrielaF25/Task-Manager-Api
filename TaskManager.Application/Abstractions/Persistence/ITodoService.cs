using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Todo.Queries;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Abstractions.Persistence
{
    public interface ITodoService
    {
        Task<TodoResponse> AddItemAsync(CreateTodoRequest todoRequest, CancellationToken ct);
        Task<bool> DeleteItemAsync(int id, CancellationToken ct);
        Task<PaginationResult<TodoResponse>> GetAllAsync(QueryParamTodo queryParam, PaginationParam pagination,CancellationToken ct);
        Task<TodoResponse?> GetByIdAsync(int id, CancellationToken ct);
        Task<TodoResponse?> UpdateItemStatusAsync(int id, CancellationToken ct);
    }
}