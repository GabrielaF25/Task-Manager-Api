namespace TaskManager.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
