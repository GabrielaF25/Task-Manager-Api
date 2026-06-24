using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Todos.Commands.DeleteTodo;

public record DeleteTodoCommand(int Id) : IRequest<Result>;