using AutoMapper;
using Task_Manager_Api.Dtos;
using Task_Manager_Api.Models;
using Task_Manager_Api.Repository;

namespace Task_Manager_Api.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;

    public TodoService(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TodoResponse>> GetAllAsync()
    {
        var items = await _todoRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<TodoResponse>>(items);
    }
    public async Task<TodoResponse?> GetByIdAsync(int id)
    {
        var item = await _todoRepository.GetByIdAsync(id);

        return _mapper.Map<TodoResponse>(item);
    }

    public async Task<TodoResponse> AddItemAsync(CreateTodoRequest todoRequest)
    {
        var createTodo = _mapper.Map<TodoItem>(todoRequest);
        createTodo.CreatedAt = DateTimeOffset.UtcNow;
        createTodo.IsCompleted = false;

        var item = await _todoRepository.AddAsync(createTodo);
        await _todoRepository.SaveChangesAsync();

        return _mapper.Map<TodoResponse>(item);
    }

    public async Task<TodoResponse?> UpdateItemStatusAsync(int id)
    {
        var item = await _todoRepository.GetByIdAsync(id);

        if (item == null)
        {
            // Nothing to update
            return null;
        }

        item.IsCompleted = true;
        await _todoRepository.SaveChangesAsync();
        return _mapper.Map<TodoResponse>(item);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _todoRepository.GetByIdAsync(id);

        if (item ==null)
        {
            return false;
        }

         _todoRepository.Remove(item);
        await _todoRepository.SaveChangesAsync();

        return true;
    }
}
