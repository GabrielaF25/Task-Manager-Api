using MediatR;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.Queries.GetTodos;

public record class GetTodosQuery(QueryParamTodo QueryParam, PaginationParam Pagination) : IRequest<Result<PaginationResult<TodoResponse>>>; 
