using System.Reflection.Metadata.Ecma335;
using TaskManager.Domain.Common;
using TaskManager.Domain.Events;

namespace TaskManager.Domain.Entities;

public class Project : Entity
{
    private Project() { }
    public static Project Create(string name, string? description, Guid ownerId)
    {
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            OwnerId = ownerId,
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project));
        return project;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;

    public ICollection<TodoItem> TodoItems { get; private set; } = [];
}
