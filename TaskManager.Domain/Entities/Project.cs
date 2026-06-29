using System.Reflection.Metadata.Ecma335;

namespace TaskManager.Domain.Entities;

public class Project
{
    private Project() { }
    public static Project Create(string name, string? description, int ownerId) =>
        new ()
        {

            Name = name,
            Description = description,
            OwnerId = ownerId,
        };
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;

    public ICollection<TodoItem> TodoItems { get; private set; } = [];
}
