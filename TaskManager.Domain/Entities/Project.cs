namespace TaskManager.Domain.Entities;

public class Project
{
    private Project() { }
    public Project(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; } 

    public ICollection<TodoItem> TodoItems { get; private set; } = new List<TodoItem>();
}
