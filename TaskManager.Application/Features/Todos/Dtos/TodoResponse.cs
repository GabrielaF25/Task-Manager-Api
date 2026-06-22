namespace TaskManager.Application.Features.Todos.Dtos;

public class TodoResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
