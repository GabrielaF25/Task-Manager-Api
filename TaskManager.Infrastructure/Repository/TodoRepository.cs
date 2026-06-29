using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Todos.GetTodos;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.ExtensionMethods;

namespace TaskManager.Infrastructure.Repository;

public class TodoRepository : ITodoRepository
{
    private readonly TaskManagerDbContext _context;

    public TodoRepository(TaskManagerDbContext managerDb)
    {
        _context = managerDb;
    }
    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        var itemTodo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id, ct);
        return itemTodo;
    }
    public async Task<PaginationResult<TodoItem>>
        GetAllAsync(QueryParamTodo queryParam,PaginationParam pagination, CancellationToken ct)
    {
        var itemsTodo =  _context.TodoItems
            .ApplyTodoFilters(queryParam)
            .ApplyTodoSorting(queryParam)
            .AsNoTracking();


        var totalCount = await itemsTodo.CountAsync(ct);

         var items = await itemsTodo
            .ApplyPagination(pagination)
            .ToListAsync(ct);

        var paginatedList = items.TransformToPageList(pagination, totalCount);

        return paginatedList;
    }

    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct)
    {
        await _context.TodoItems.AddAsync(item, ct);
        return item;
    }

    public void Remove(TodoItem item)
    {
        _context.TodoItems.Remove(item);

    }
}
