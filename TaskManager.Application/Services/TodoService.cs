using AutoMapper;
using FluentValidation;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Todo.Queries;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTodoRequest> _validator;
    private readonly IValidator<PaginationParam> _validatorResult;

    public TodoService(ITodoRepository todoRepository, IMapper mapper, IValidator<CreateTodoRequest> validator, IValidator<PaginationParam> param)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
        _validator = validator;
        _validatorResult = param;
    }

    public async Task<PaginationResult<TodoResponse>> 
        GetAllAsync(QueryParamTodo queryParam,PaginationParam pagination,CancellationToken ct)
    {
        await _validatorResult.ValidateAndThrowAsync(pagination, ct);
        var itemsPageResult = await _todoRepository.GetAllAsync(queryParam,pagination,ct);

        var pageResult = new PaginationResult<TodoResponse>()
        {
            PageNumber = itemsPageResult.PageNumber,
            PageSize = itemsPageResult.PageSize,
            TotalCount = itemsPageResult.TotalCount,
            TotalPages = itemsPageResult.TotalPages,
            HasNextPage = itemsPageResult.HasNextPage,
            HasPreviousPage = itemsPageResult.HasPreviousPage,
            Items = _mapper.Map<IEnumerable<TodoResponse>>(itemsPageResult.Items)
        };

        return pageResult;
    }
    public async Task<TodoResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(id, ct);

        return _mapper.Map<TodoResponse>(item);
    }

    public async Task<TodoResponse> AddItemAsync(CreateTodoRequest todoRequest, CancellationToken ct)
    {
         await _validator.ValidateAndThrowAsync(todoRequest, ct);

        var createTodo = _mapper.Map<TodoItem>(todoRequest);

        createTodo.CreatedAt = DateTimeOffset.UtcNow;
        createTodo.IsCompleted = false;

        var item = await _todoRepository.AddAsync(createTodo, ct);
        await _todoRepository.SaveChangesAsync(ct);

        return _mapper.Map<TodoResponse>(item);
    }

    public async Task<TodoResponse?> UpdateItemStatusAsync(int id, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(id, ct);

        if (item == null)
        {
            return null;
        }

        item.IsCompleted = true;
        await _todoRepository.SaveChangesAsync(ct);
        return _mapper.Map<TodoResponse>(item);
    }

    public async Task<bool> DeleteItemAsync(int id, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(id, ct);

        if (item ==null)
        {
            return false;
        }

         _todoRepository.Remove(item);
        await _todoRepository.SaveChangesAsync(ct);

        return true;
    }
}
