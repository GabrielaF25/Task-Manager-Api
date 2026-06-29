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
    private readonly IValidator<CreateTodoRequest> _validator;

    public CreateTodoCommandHandler(
        ITodoRepository todoRepository,
        IMapper mapper,
        IValidator<CreateTodoRequest> validator)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<TodoResponse>> Handle(CreateTodoCommand todoRequest, CancellationToken ct)
    {
        var resultValidator = await _validator.ValidateAsync(todoRequest.TodoRequest, ct);

        if (!resultValidator.IsValid)
        {
            var errors = resultValidator.Errors.Select(x => x.ErrorMessage).ToList();

            return Result<TodoResponse>.Failed(errors, StatusType.ValidationError);
        }

        var createTodo = TodoItem.Create(todoRequest.TodoRequest.Title, todoRequest.TodoRequest.Description, todoRequest.TodoRequest.ProjectId);

        createTodo.Complete();

        var item = await _todoRepository.AddAsync(createTodo, ct);
        await _todoRepository.SaveChangesAsync(ct);

        return Result<TodoResponse>.Success(_mapper.Map<TodoResponse>(item));
    }

}
