using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Authetication;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Users.CreateUser;

[TestFixture]
public class CreateUserCommandHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IPasswordHasherService> _passwordHasherServiceMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private CreateUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherServiceMock = new Mock<IPasswordHasherService>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherServiceMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task Handle_Should_Create_User_With_Normalized_Email()
    {
        // Arrange
        var command = new CreateUserCommand(new CreateUserRequest
        {
            UserName = "gabi",
            Email = " TEST@EMAIL.COM ",
            Password = "Password123"
        });

        _passwordHasherServiceMock
            .Setup(x => x.HashPassword(It.IsAny<User>(), "Password123"))
            .Returns("hashed-password");

        _userRepositoryMock
            .Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(new UserResponse
            {
                UserName = "gabi",
                Email = "test@email.com"
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _userRepositoryMock.Verify(x =>
            x.CreateUserAsync(
                It.Is<User>(u =>
                    u.UserName == "gabi" &&
                    u.Email == "test@email.com" &&
                    u.PasswordHash == "hashed-password"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_Should_Hash_Password_Before_Saving_User()
    {
        // Arrange
        var command = new CreateUserCommand(new CreateUserRequest
        {
            UserName = "gabi",
            Email = "test@email.com",
            Password = "Password123"
        });

        _passwordHasherServiceMock
            .Setup(x => x.HashPassword(It.IsAny<User>(), "Password123"))
            .Returns("hashed-password");

        _userRepositoryMock
            .Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(new UserResponse
            {
                UserName = "gabi",
                Email = "test@email.com"
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _passwordHasherServiceMock.Verify(
            x => x.HashPassword(
                It.Is<User>(u =>
                    u.Email == "test@email.com" &&
                    u.UserName == "gabi"),
                "Password123"),
            Times.Once);
    }

    [Test]
    public async Task Handle_Should_Return_UserResponse_When_User_Is_Created()
    {
        // Arrange
        var command = new CreateUserCommand(new CreateUserRequest
        {
            UserName = "gabi",
            Email = "test@email.com",
            Password = "Password123"
        });

        var userResponse = new UserResponse
        {
            UserName = "gabi",
            Email = "test@email.com"
        };

        _passwordHasherServiceMock
            .Setup(x => x.HashPassword(It.IsAny<User>(), "Password123"))
            .Returns("hashed-password");

        _userRepositoryMock
            .Setup(x => x.CreateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(It.IsAny<User>()))
            .Returns(userResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(userResponse);

        _mapperMock.Verify(
            x => x.Map<UserResponse>(It.IsAny<User>()),
            Times.Once);
    }
}