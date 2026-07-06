using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Users.UpdateUserRole;

public class UpdateUserRequest
{
    public int Id {  get; set; }
    public UserRole Role { get; set; }
}
