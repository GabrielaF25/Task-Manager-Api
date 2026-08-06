using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Application.Tests.Features.Users.CreateUser;

[TestFixture]
public class CreateUserValidationTests
{
    private Mock<IUserLookupService> _userLookupServiceMock = null!;
    private CreateUserCommandValidation _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _userLookupServiceMock = new Mock<IUserLookupService>();
        _validator = new CreateUserCommandValidation(_userLookupServiceMock.Object);
    }

    private static CreateUserCommand CreateValidCommand()
    {
        return new CreateUserCommand(
            new CreateUserRequest
            {
                UserName = "Gabriel",
                Email = "gabriel@test.com",
                Password = "Password123"
            });
    }

    [Test]
    public async Task Should_Have_Error_When_UserName_Already_Exists()
    {
        var command = CreateValidCommand();

        _userLookupServiceMock
            .Setup(x => x.UserNameExistsAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserToCreate.UserName)
            .WithErrorMessage("UserName is already registered");
    }

    [Test]
    public async Task Should_Have_Error_When_Email_Already_Exists()
    {
        var command = CreateValidCommand();

        _userLookupServiceMock
            .Setup(x => x.EmailExistsAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserToCreate.Email)
            .WithErrorMessage("Email is already registered");
    }

    [Test]
    public async Task Should_Not_Have_Any_Errors_When_Command_Is_Valid()
    {
        var command = CreateValidCommand();

        _userLookupServiceMock
            .Setup(x => x.UserNameExistsAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userLookupServiceMock
            .Setup(x => x.EmailExistsAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task Should_Normalize_UserName_Before_Checking_If_Exists()
    {
        var command = CreateValidCommand();
        command.UserToCreate.UserName = "  Gabriel  ";

        await _validator.TestValidateAsync(command);

        _userLookupServiceMock.Verify(x =>
            x.UserNameExistsAsync(
                "gabriel",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_Normalize_Email_Before_Checking_If_Exists()
    {
        var command = CreateValidCommand();
        command.UserToCreate.Email = "  Gabriel@Test.com  ";

        await _validator.TestValidateAsync(command);

        _userLookupServiceMock.Verify(x =>
            x.EmailExistsAsync(
                "gabriel@test.com",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}