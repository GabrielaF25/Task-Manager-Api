namespace TaskManager.Application.Features.Users.Dtos;

public class UserResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
