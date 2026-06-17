namespace Task_Manager_Api.Dtos;

public class TodoResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
