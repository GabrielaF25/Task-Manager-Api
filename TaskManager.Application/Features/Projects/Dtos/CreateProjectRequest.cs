
namespace TaskManager.Application.Features.Projects.Dto;

public class CreateProjectRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
