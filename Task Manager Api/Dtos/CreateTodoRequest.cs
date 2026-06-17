using System.ComponentModel.DataAnnotations;

namespace Task_Manager_Api.Dtos;

public class CreateTodoRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
