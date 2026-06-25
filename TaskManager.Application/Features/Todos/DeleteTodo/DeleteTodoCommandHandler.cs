using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Todos.DeleteTodo;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Result>
{
    private readonly ITodoRepository _todoRepository;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result> Handle(DeleteTodoCommand deleteTodoCommand, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(deleteTodoCommand.Id, ct);

        if (item == null)
        {
            var error = new List<string> { "The todo item was not found." };
            return Result.Failed(error, StatusType.NotFound);
        }

        _todoRepository.Remove(item);
        await _todoRepository.SaveChangesAsync(ct);

        return Result.Success();
    }
}
