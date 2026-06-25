namespace TaskManager.Domain.Entities;

public class TodoItem
{
    private TodoItem() { }
    public static TodoItem Create(string title, string? description, int projectId)
    {
        return new TodoItem
        {
            Title = title,
            Description = description,
            ProjectId = projectId,
            CreatedAt = DateTimeOffset.UtcNow,
            IsCompleted = false
        };
    }
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsCompleted {  get; private set; }
    public int ProjectId {  get; private set; }
    public Project Project { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public void Complete( )
    {
        IsCompleted = true;
    }
}
