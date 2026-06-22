using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Domain.Entities;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsCompleted {  get; set; }

    public int ProjectId {  get; set; }
    public Project Project { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}
