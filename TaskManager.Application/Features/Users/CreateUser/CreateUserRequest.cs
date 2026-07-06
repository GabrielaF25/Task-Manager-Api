using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.Features.Users.CreateUser;

public class CreateUserRequest
{
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
}
