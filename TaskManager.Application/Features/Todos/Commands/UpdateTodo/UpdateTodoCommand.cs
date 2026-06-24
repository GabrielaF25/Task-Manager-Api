using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.Commands.UpdateTodo;

public record UpdateTodoCommand(int Id) : IRequest<Result<TodoResponse>>;
