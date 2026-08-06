using FluentAssertions;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Tests.Entities;

[TestFixture]
public class RefreshTokenTests
{
    [Test]
    public void Create_Should_Set_Properties()
    {
        // Arrange
        var token = "refresh-token";
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        var refreshToken = RefreshToken.Create(token, expiresAt);

        // Assert
        refreshToken.Token.Should().Be(token);
        refreshToken.ExpiresAt.Should().Be(expiresAt);
    }

    [Test]
    public void Create_Should_Set_CreatedAt()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var refreshToken = RefreshToken.Create("refresh-token", DateTimeOffset.UtcNow.AddDays(7));

        var after = DateTimeOffset.UtcNow;

        // Assert
        refreshToken.CreatedAt.Should().BeOnOrAfter(before);
        refreshToken.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Test]
    public void Create_Should_Be_Active()
    {
        // Arrange & Act
        var refreshToken = RefreshToken.Create("refresh-token", DateTimeOffset.UtcNow.AddDays(7));

        // Assert
        refreshToken.IsActive.Should().BeTrue();
        refreshToken.IsExpired.Should().BeFalse();
        refreshToken.IsRevoked.Should().BeFalse();
    }

    [Test]
    public void Create_Should_Be_Expired_When_Expiration_Date_Is_In_The_Past()
    {
        // Arrange & Act
        var refreshToken = RefreshToken.Create("refresh-token", DateTimeOffset.UtcNow.AddDays(-1));

        // Assert
        refreshToken.IsExpired.Should().BeTrue();
        refreshToken.IsActive.Should().BeFalse();
    }

    [Test]
    public void Revoke_Should_Set_RevokedAt()
    {
        // Arrange
        var refreshToken = RefreshToken.Create("refresh-token", DateTimeOffset.UtcNow.AddDays(7));

        // Act
        refreshToken.Revoke();

        // Assert
        refreshToken.RevokedAt.Should().NotBeNull();
        refreshToken.IsRevoked.Should().BeTrue();
    }

    [Test]
    public void Revoke_Should_Make_Token_Inactive()
    {
        // Arrange
        var refreshToken = RefreshToken.Create("refresh-token", DateTimeOffset.UtcNow.AddDays(7));

        // Act
        refreshToken.Revoke();

        // Assert
        refreshToken.IsActive.Should().BeFalse();
    }
}