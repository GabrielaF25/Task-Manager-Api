using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Application.Features.Todos.Queries.GetTodo;

namespace TaskManager.Application.Features.Todos.GetTodo;

public class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, Result<TodoResponse>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;

    public GetTodoQueryHandler(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<Result<TodoResponse>> Handle(GetTodoQuery todoByIdCommand, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(todoByIdCommand.Id, ct);

        if (item == null)
        {
            var errors = new List<string> { "The todo item was not found." };
            return Result<TodoResponse>.Failed(errors, StatusType.NotFound);
        }

        return Result<TodoResponse>.Success(_mapper.Map<TodoResponse>(item));
    }

}
