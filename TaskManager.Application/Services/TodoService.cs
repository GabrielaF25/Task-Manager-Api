using AutoMapper;
using FluentValidation;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todo.Queries;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTodoRequest> _validator;
    private readonly IValidator<PaginationParam> _paginationValidator;

    public TodoService(ITodoRepository todoRepository, IMapper mapper, IValidator<CreateTodoRequest> validator, IValidator<PaginationParam> param)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
        _validator = validator;
        _paginationValidator = param;
    }

    public async Task<Result<PaginationResult<TodoResponse>>> GetAllAsync(QueryParamTodo queryParam,PaginationParam pagination,CancellationToken ct)
    {
        var validationResult = await _paginationValidator.ValidateAsync(pagination, ct);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            return Result<PaginationResult<TodoResponse>>.Failed(
                errors,
                StatusType.ValidationError);
        }
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

        return Result<PaginationResult<TodoResponse>>.Success(pageResult);
    }
    public async Task<Result<TodoResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(id, ct);

        if (item == null)
        {
            var errors = new List<string> { "The todo item was not found." };
            return Result<TodoResponse>.Failed(errors, StatusType.NotFound);
        }

        return Result<TodoResponse>.Success(_mapper.Map<TodoResponse>(item));
    }

    public async Task<Result<TodoResponse>> AddItemAsync(CreateTodoRequest todoRequest, CancellationToken ct)
    {
        var resultValidator =  await _validator.ValidateAsync(todoRequest, ct);

        if (!resultValidator.IsValid)
        {
            var errors = resultValidator.Errors.Select(x =>x.ErrorMessage).ToList();

            return Result<TodoResponse>.Failed(errors, StatusType.ValidationError);
        }

        var createTodo = _mapper.Map<TodoItem>(todoRequest);

        createTodo.CreatedAt = DateTimeOffset.UtcNow;
        createTodo.IsCompleted = false;

        var item = await _todoRepository.AddAsync(createTodo, ct);
        await _todoRepository.SaveChangesAsync(ct);

        return  Result<TodoResponse>.Success( _mapper.Map<TodoResponse>(item));
    }

    public async Task<Result<TodoResponse>> UpdateItemStatusAsync(int id, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(id, ct);

        if (item == null)
        {
            var error = new List<string> { "The todo item was not found." };
            return Result<TodoResponse>.Failed(error, StatusType.NotFound);
        }

        item.IsCompleted = true;
        await _todoRepository.SaveChangesAsync(ct);
        return Result<TodoResponse>.Success(_mapper.Map<TodoResponse>(item));
    }

    public async Task<Result> DeleteItemAsync(int id, CancellationToken ct)
    {
        var item = await _todoRepository.GetByIdAsync(id, ct);

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
