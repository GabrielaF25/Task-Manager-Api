using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Events;

namespace TaskManager.Domain.Entities;

public class User : Entity
{
    private User() { }
    public static User Register(string email, string userName)
    {
        var user =  new User() {
            Email = email,
            UserName = userName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id));

        return user;
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
