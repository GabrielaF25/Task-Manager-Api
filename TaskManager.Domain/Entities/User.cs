using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class User
{
    private User() { }
    public static User Register(string email, string userName)
    {
        return new User() {
            Email = email,
            UserName = userName,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public int Id { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public UserRole UserRole{get; private set;} = UserRole.User;
    public string PasswordHash {  get; private set; } = null!;
    public DateTimeOffset CreatedAt {  get; private set; }
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        RefreshTokens.Add(refreshToken);
    }

    public void ChangeRole(UserRole userRole)
    {
        UserRole = userRole;
    }

}
