using Task_Manager_Api.Dtos;
using Task_Manager_Api.Models;

namespace Task_Manager_Api.Repository;

public interface ITodoRepository
{
    Task<TodoItem> AddAsync(TodoItem item);
    void Remove(TodoItem item);
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task SaveChangesAsync();
}