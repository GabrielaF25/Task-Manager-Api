using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.Features.Authentication.Dtos;

public class UserCredentials
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}
