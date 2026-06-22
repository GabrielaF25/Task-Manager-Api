using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.Features.Todos.Dtos;

public class CreateTodoRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
