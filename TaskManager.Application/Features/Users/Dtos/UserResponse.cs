using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Users.Dtos;

public class UserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRole UserRole { get; set; }
}
