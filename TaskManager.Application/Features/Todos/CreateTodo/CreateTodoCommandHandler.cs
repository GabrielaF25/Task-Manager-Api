using AutoMapper;
using FluentValidation;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Todos.CreateTodo;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand,Result<TodoResponse>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;

    public CreateTodoCommandHandler(
        ITodoRepository todoRepository,
        IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<Result<TodoResponse>> Handle(CreateTodoCommand todoRequest, CancellationToken ct)
    {
        var createTodo = TodoItem.Create(todoRequest.TodoRequest.Title, todoRequest.TodoRequest.Description, todoRequest.TodoRequest.ProjectId);

        createTodo.Complete();

        var item = await _todoRepository.AddAsync(createTodo, ct);

        return Result<TodoResponse>.Success(_mapper.Map<TodoResponse>(item));
    }

}
