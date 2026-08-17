using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.UpdateTodo;

public record UpdateTodoCommand(Guid Id) : IRequest<Result<TodoResponse>>;
