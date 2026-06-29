using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.UpdateTodo;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, Result<TodoResponse>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;

    public UpdateTodoCommandHandler(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }


    public async Task<Result<TodoResponse>> Handle(UpdateTodoCommand updateTodoCommand, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(updateTodoCommand.Id, ct);

        if (item == null)
        {
            var error = new List<string> { "The todo item was not found." };
            return Result<TodoResponse>.Failed(error, StatusType.NotFound);
        }

        item.Complete();

        return Result<TodoResponse>.Success(_mapper.Map<TodoResponse>(item));
    }
}
