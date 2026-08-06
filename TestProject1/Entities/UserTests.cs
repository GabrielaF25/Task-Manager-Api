using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Events;

namespace TaskManager.Domain.Tests.Entities;

[TestFixture]
public class UserTests
{
    [Test]
    public void Register_Should_Set_User_Properties()
    {
        var email = "test@email.com";
        var userName = "gabi";

        var user = User.Register(email, userName);

        user.Email.Should().Be(email);
        user.UserName.Should().Be(userName);
        user.UserRole.Should().Be(UserRole.User);
    }

    [Test]
    public void Register_Should_Set_CreatedAt()
    {
        var before = DateTimeOffset.UtcNow;

        var user = User.Register("test@email.com", "gabi");

        var after = DateTimeOffset.UtcNow;

        user.CreatedAt.Should().BeOnOrAfter(before);
        user.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Test]
    public void Register_Should_Initialize_RefreshTokens_As_Empty_Collection()
    {
        var user = User.Register("test@email.com", "gabi");

        user.RefreshTokens.Should().NotBeNull();
        user.RefreshTokens.Should().BeEmpty();
    }

    [Test]
    public void Register_Should_Add_UserRegisteredEvent()
    {
        var user = User.Register("test@email.com", "gabi");

        user.DomainEvents.Should().ContainSingle();
        user.DomainEvents.First().Should().BeOfType<UserRegisteredEvent>();
    }

    [Test]
    public void Register_Should_Add_UserRegisteredEvent_With_Created_User()
    {
        var user = User.Register("test@email.com", "gabi");

        var domainEvent = user.DomainEvents
            .OfType<UserRegisteredEvent>()
            .Single();

        domainEvent.User.Should().Be(user);
    }

    [Test]
    public void SetPasswordHash_Should_Set_PasswordHash()
    {
        var user = User.Register("test@email.com", "gabi");
        var hash = "hashed_password";

        user.SetPasswordHash(hash);

        user.PasswordHash.Should().Be(hash);
    }

    [Test]
    public void ChangeRole_Should_Change_UserRole()
    {
        var user = User.Register("test@email.com", "gabi");

        user.ChangeRole(UserRole.Admin);

        user.UserRole.Should().Be(UserRole.Admin);
    }

    [Test]
    public void AddRefreshToken_Should_Add_Token_To_RefreshTokens()
    {
        var user = User.Register("test@email.com", "gabi");

        var refreshToken = RefreshToken.Create(
            token: "refresh-token",
            expiresAt: DateTimeOffset.UtcNow.AddDays(7));

        user.AddRefreshToken(refreshToken);

        user.RefreshTokens.Should().ContainSingle();
        user.RefreshTokens.Should().Contain(refreshToken);
    }
}