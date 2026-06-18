using AutoMapper;
using FluentValidation;
using Task_Manager_Api.Models;
using TaskManager.Application.Abstraction;
using TaskManager.Application.Features.Todo.Dtos;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Pagination;
using TaskManager.Domain.Pagination;

namespace Task_Manager_Api.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTodoRequest> _validator;

    public TodoService(ITodoRepository todoRepository, IMapper mapper, IValidator<CreateTodoRequest> validator)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<IEnumerable<TodoResponse>> 
        GetAllAsync(QueryParamTodo queryParam,PaginationParam pagination,CancellationToken ct)
    {
        var items = await _todoRepository.GetAllAsync(queryParam,pagination,ct);

        return _mapper.Map<IEnumerable<TodoResponse>>(items);
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
