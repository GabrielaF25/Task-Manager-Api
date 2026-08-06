using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Authentication.Logout;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Authentication.Logout;

[TestFixture]
public class LogoutUserCommandHandlerTests
{
    private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = null!;
    private LogoutUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _handler = new LogoutUserCommandHandler(_refreshTokenRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_Forbidden_When_RefreshToken_Does_Not_Exist()
    {
        // Arrange
        var command = new LogoutUserCommand(new RefreshTokenRequest
        {
            RefreshToken = "invalid-refresh-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("invalid-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Forbidden);
        result.Errors.Should().Contain("Invalid refresh token.");
    }

    [Test]
    public async Task Handle_Should_Return_Forbidden_When_RefreshToken_Is_Revoked()
    {
        // Arrange
        var refreshToken = RefreshToken.Create(
            token: "refresh-token",
            expiresAt: DateTimeOffset.UtcNow.AddDays(7));

        refreshToken.Revoke();

        var command = new LogoutUserCommand(new RefreshTokenRequest
        {
            RefreshToken = "refresh-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Forbidden);
        result.Errors.Should().Contain("Invalid refresh token.");
    }

    [Test]
    public async Task Handle_Should_Return_Forbidden_When_RefreshToken_Is_Expired()
    {
        // Arrange
        var refreshToken = RefreshToken.Create(
            token: "refresh-token",
            expiresAt: DateTimeOffset.UtcNow.AddDays(-1));

        var command = new LogoutUserCommand(new RefreshTokenRequest
        {
            RefreshToken = "refresh-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Forbidden);
        result.Errors.Should().Contain("Invalid refresh token.");
    }

    [Test]
    public async Task Handle_Should_Revoke_RefreshToken_When_RefreshToken_Is_Valid()
    {
        // Arrange
        var refreshToken = RefreshToken.Create(
            token: "refresh-token",
            expiresAt: DateTimeOffset.UtcNow.AddDays(7));

        var command = new LogoutUserCommand(new RefreshTokenRequest
        {
            RefreshToken = "refresh-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        refreshToken.IsRevoked.Should().BeTrue();
        refreshToken.IsActive.Should().BeFalse();
        refreshToken.RevokedAt.Should().NotBeNull();
    }
}