using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Todos.DeleteTodo;

public record DeleteTodoCommand(int Id) : IRequest<Result>;