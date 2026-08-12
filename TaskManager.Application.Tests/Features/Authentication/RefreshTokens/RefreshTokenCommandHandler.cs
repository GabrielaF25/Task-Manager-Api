using FluentAssertions;
using Moq;
using System.Reflection;
using TaskManager.Application.Abstractions.Authetication;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Authentication.RefreshTokens;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Authentication.RefreshTokens;

[TestFixture]
public class RefreshTokenCommandHandlerTests
{
    private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = null!;
    private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = null!;
    private RefreshTokenCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _handler = new RefreshTokenCommandHandler(
            _refreshTokenRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_Unauthorized_When_RefreshToken_Does_Not_Exist()
    {
        var command = new RefreshTokenCommand(new RefreshTokenRequest
        {
            RefreshToken = "invalid-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("invalid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Unauthorized);
        result.Errors.Should().Contain("Invalid refresh token.");
    }

    [Test]
    public async Task Handle_Should_Return_Unauthorized_When_RefreshToken_Is_Expired()
    {
        var refreshToken = RefreshToken.Create(
            "old-refresh-token",
            DateTimeOffset.UtcNow.AddDays(-1));

        var command = new RefreshTokenCommand(new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Unauthorized);
        result.Errors.Should().Contain("Invalid refresh token.");
    }

    [Test]
    public async Task Handle_Should_Return_Unauthorized_When_RefreshToken_Is_Revoked()
    {
        var refreshToken = RefreshToken.Create(
            "old-refresh-token",
            DateTimeOffset.UtcNow.AddDays(7));

        refreshToken.Revoke();

        var command = new RefreshTokenCommand(new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Unauthorized);
        result.Errors.Should().Contain("Invalid refresh token.");
    }

    [Test]
    public async Task Handle_Should_Revoke_Old_Token_And_Return_New_Tokens_When_RefreshToken_Is_Valid()
    {
        var user = User.Register("test@email.com", "gabi");

        var existingRefreshToken = RefreshToken.Create(
            "old-refresh-token",
            DateTimeOffset.UtcNow.AddDays(7));

        SetPrivateProperty(existingRefreshToken, nameof(RefreshToken.User), user);

        var command = new RefreshTokenCommand(new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        });

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateJwt(user))
            .Returns("new-access-token");

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        _jwtTokenGeneratorMock
            .Setup(x => x.GetRefreshTokenExpiration())
            .Returns(DateTimeOffset.UtcNow.AddDays(7));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        result.Data!.AccessToken.Should().Be("new-access-token");
        result.Data.RefreshToken.Should().Be("new-refresh-token");

        existingRefreshToken.IsRevoked.Should().BeTrue();

        user.RefreshTokens.Should().ContainSingle();
        user.RefreshTokens.First().Token.Should().Be("new-refresh-token");
    }

    private static void SetPrivateProperty<T>(
        object obj,
        string propertyName,
        T value)
    {
        var property = obj.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        property!.SetValue(obj, value);
    }
}