using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class TodoItem : Entity
{
    private TodoItem() { }
    public static TodoItem Create(string title, string? description, Guid projectId)
    {
        return new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            ProjectId = projectId,
            CreatedAt = DateTimeOffset.UtcNow,
            IsCompleted = false
        };
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsCompleted {  get; private set; }
    public Guid ProjectId {  get; private set; }
    public Project Project { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public void Complete( )
    {
        IsCompleted = true;
    }
}
