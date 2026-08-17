using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Todos.DeleteTodo;

public record DeleteTodoCommand(Guid Id) : IRequest<Result>;