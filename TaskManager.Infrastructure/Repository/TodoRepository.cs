using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Task_Manager_Api.DbContexts;
using Task_Manager_Api.Models;
using TaskManager.Application.Abstraction;
using TaskManager.Application.Pagination;
using TaskManager.Domain.Pagination;
using TaskManager.Infrastructure.ExtensionMethods;

namespace Task_Manager_Api.Repository;

public class TodoRepository : ITodoRepository
{
    private readonly TaskManagerDb _context;

    public TodoRepository(TaskManagerDb managerDb)
    {
        _context = managerDb;
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        var itemTodo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id, ct);
        return itemTodo;
    }
    public async Task<IEnumerable<TodoItem>>
        GetAllAsync(QueryParamTodo queryParam,PaginationParam pagination, CancellationToken ct)
    {
        var itemTodo = await _context.TodoItems
            .Filter(queryParam)
            .ApplyPagination(pagination)
            .AsNoTracking()
            .ToListAsync(ct);

        return itemTodo;
    }

    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct)
    {
        await _context.TodoItems.AddAsync(item, ct);
        return item;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }

    public void Remove(TodoItem item)
    {
        _context.TodoItems.Remove(item);

    }

}
