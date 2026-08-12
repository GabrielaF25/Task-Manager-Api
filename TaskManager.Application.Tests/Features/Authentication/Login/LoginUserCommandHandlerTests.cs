using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Authetication;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Authentication.Login;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Authentication.Login;

[TestFixture]
public class LoginUserCommandHandlerTests
{
    private Mock<IPasswordHasherService> _passwordHasherServiceMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = null!;
    private LoginUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _passwordHasherServiceMock = new Mock<IPasswordHasherService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _handler = new LoginUserCommandHandler(
            _passwordHasherServiceMock.Object,
            _userRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_Unauthorized_When_User_Does_Not_Exist()
    {
        // Arrange
        var command = new LoginUserCommand(new UserCredentials
        {
            Email = "test@email.com",
            Password = "Password123"
        });

        _userRepositoryMock
            .Setup(x => x.GetUserByUseEmail("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Unauthorized);
        result.Errors.Should().Contain("Invalid credentials.");

        _passwordHasherServiceMock.Verify(
            x => x.VerifyPassword(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _jwtTokenGeneratorMock.Verify(
            x => x.GenerateJwt(It.IsAny<User>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_Unauthorized_When_Password_Is_Invalid()
    {
        // Arrange
        var user = User.Register("test@email.com", "gabi");
        user.SetPasswordHash("hashed-password");

        var command = new LoginUserCommand(new UserCredentials
        {
            Email = "test@email.com",
            Password = "wrong-password"
        });

        _userRepositoryMock
            .Setup(x => x.GetUserByUseEmail("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherServiceMock
            .Setup(x => x.VerifyPassword(user, "hashed-password", "wrong-password"))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Unauthorized);
        result.Errors.Should().Contain("Invalid credentials.");

        _jwtTokenGeneratorMock.Verify(
            x => x.GenerateJwt(It.IsAny<User>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_LoginResponse_When_Credentials_Are_Valid()
    {
        // Arrange
        var user = User.Register("test@email.com", "gabi");
        user.SetPasswordHash("hashed-password");

        var command = new LoginUserCommand(new UserCredentials
        {
            Email = " TEST@EMAIL.COM ",
            Password = "Password123"
        });

        _userRepositoryMock
            .Setup(x => x.GetUserByUseEmail("test@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherServiceMock
            .Setup(x => x.VerifyPassword(user, "hashed-password", "Password123"))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateJwt(user))
            .Returns("access-token");

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        _jwtTokenGeneratorMock
            .Setup(x => x.GetRefreshTokenExpiration())
            .Returns(DateTimeOffset.UtcNow.AddDays(7));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data!.UserName.Should().Be("gabi");
        result.Data.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");

        user.RefreshTokens.Should().ContainSingle();
        user.RefreshTokens.First().Token.Should().Be("refresh-token");
    }
}