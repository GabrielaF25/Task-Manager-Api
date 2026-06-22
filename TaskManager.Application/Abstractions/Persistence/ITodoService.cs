using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todo.Queries;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Abstractions.Persistence
{
    public interface ITodoService
    {
        Task<Result<TodoResponse>> AddItemAsync(CreateTodoRequest todoRequest, CancellationToken ct);
        Task<Result> DeleteItemAsync(int id, CancellationToken ct);
        Task<Result<PaginationResult<TodoResponse>>> GetAllAsync(QueryParamTodo queryParam, PaginationParam pagination,CancellationToken ct);
        Task<Result<TodoResponse>> GetByIdAsync(int id, CancellationToken ct);
        Task<Result<TodoResponse>> UpdateItemStatusAsync(int id, CancellationToken ct);
    }
}