using Microsoft.EntityFrameworkCore;
using Task_Manager_Api.DbContexts;
using Task_Manager_Api.Models;

namespace Task_Manager_Api.Repository;

public class TodoRepository : ITodoRepository
{
    private readonly TaskManagerDb _context;

    public TodoRepository(TaskManagerDb managerDb)
    {
        _context = managerDb;
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        var itemTodo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
        return itemTodo;
    }
    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        var itemTodo = await _context.TodoItems.ToListAsync() ;
        return itemTodo;
    }

    public async Task<TodoItem> AddAsync(TodoItem item)
    {
        await _context.TodoItems.AddAsync(item);
        return  item;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void  Remove( TodoItem item)
    {  
        _context.TodoItems.Remove(item);
        
    }
}
