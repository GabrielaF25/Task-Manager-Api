using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Users.UpdateUserRole;

public class UpdateUserRequest
{
    public Guid Id {  get; set; }
    public UserRole Role { get; set; }
}
