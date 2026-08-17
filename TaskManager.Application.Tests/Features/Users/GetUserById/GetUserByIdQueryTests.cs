using AutoMapper;
using Moq;
using NUnit.Framework;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Application.Features.Users.GetUser;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Users.GetUser;

[TestFixture]
public class GetUserByIdQueryHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private GetUserByIdQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetUserByIdQueryHandler(
            _userRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.NewGuid());

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusType, Is.EqualTo(StatusType.NotFound));
        Assert.That(result.Errors, Does.Contain(" The user was not found."));

        _mapperMock.Verify(
            x => x.Map<UserResponse>(It.IsAny<User>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_User_When_User_Exists()
    {
        // Arrange
        var user = User.Register(
            "gabriel@test.com",
            "Gabriel");

        var response = new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            UserRole = user.UserRole
        };

        var query = new GetUserByIdQuery(user.Id);

        _userRepositoryMock
            .Setup(x => x.GetUserByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<UserResponse>(user))
            .Returns(response);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Id, Is.EqualTo(response.Id));
        Assert.That(result.Data.UserName, Is.EqualTo(response.UserName));
        Assert.That(result.Data.Email, Is.EqualTo(response.Email));
        Assert.That(result.Data.UserRole, Is.EqualTo(response.UserRole));
            
        _userRepositoryMock.Verify(
            x => x.GetUserByIdAsync(query.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<UserResponse>(user),
            Times.Once);
    }
}