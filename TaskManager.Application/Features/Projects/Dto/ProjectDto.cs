
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Projects.Dto;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 

    public ICollection<TodoResponse> TodoItems { get; set; } = new List<TodoResponse>();
}
