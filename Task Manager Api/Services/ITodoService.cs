using Task_Manager_Api.Dtos;

namespace Task_Manager_Api.Services
{
    public interface ITodoService
    {
        Task<TodoResponse> AddItemAsync(CreateTodoRequest todoRequest);
        Task<bool> DeleteItem(int id);
        Task<IEnumerable<TodoResponse>> GetAllAsync();
        Task<TodoResponse?> GetByIdAsync(int id);
        Task<TodoResponse?> UpdateItemStatusAsync(int id);
    }
}