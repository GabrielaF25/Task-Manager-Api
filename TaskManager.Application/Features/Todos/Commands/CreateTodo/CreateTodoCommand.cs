using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.Commands.CreateTodo;

public record CreateTodoCommand(CreateTodoRequest TodoRequest) : IRequest<Result<TodoResponse>>;
