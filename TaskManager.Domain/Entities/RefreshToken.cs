namespace TaskManager.Domain.Entities;

public class RefreshToken
{
    public int Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    public static RefreshToken Create(string token, DateTimeOffset expiresAt)
    {
        return new RefreshToken
        {
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow
        };
    } 
    public void Revoke()
    {
        RevokedAt = DateTimeOffset.UtcNow;
    }
}
