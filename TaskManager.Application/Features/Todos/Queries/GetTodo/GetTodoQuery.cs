using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.Queries.GetTodo;

public record class GetTodoQuery(int Id) :IRequest<Result<TodoResponse>>;
