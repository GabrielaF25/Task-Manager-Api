using AutoMapper;
using Moq;
using NUnit.Framework;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Application.Features.Users.UpdateUserRole;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tests.Features.Users.UpdateUserRole;

[TestFixture]
public class UpdateUserRoleCommandHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<ICurrentUserService> _currentUserServiceMock = null!;
    private UpdateUserRoleCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _mapperMock = new Mock<IMapper>();

        _handler = new UpdateUserRoleCommandHandler(
            _userRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            Role = UserRole.Admin
        };
        var user = User.Register("user@user.ro", "UserName", UserRole.Admin);

        var command = new UpdateUserRoleCommand(request);

        var userId = Guid.NewGuid();
        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
           .Setup(x => x.GetUserByIdAsync(request.Id, It.IsAny<CancellationToken>()))
           .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusType, Is.EqualTo(StatusType.NotFound));
        Assert.That(result.Errors, Does.Contain("User not found"));

        _mapperMock.Verify(
            x => x.Map<UserResponse>(It.IsAny<User>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Update_User_Role_And_Return_Success()
    {
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            Role = UserRole.Admin
        };
        var user = User.Register("user@user.ro", "UserName", UserRole.Admin);

        var userId = Guid.NewGuid();
        var command = new UpdateUserRoleCommand(request);

        var response = new UserResponse
        {
            Id = request.Id,
            UserName = user.UserName,
            Email = user.Email,
            UserRole = UserRole.Admin
        };

        _currentUserServiceMock
           .Setup(x => x.GetCurrentUserId())
           .Returns(userId);

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(user))
            .Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(user.UserRole, Is.EqualTo(UserRole.Admin));
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.UserRole, Is.EqualTo(UserRole.Admin));

        _userRepositoryMock.Verify(
            x => x.GetUserByIdAsync(request.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<UserResponse>(user),
            Times.Once);
    }
}