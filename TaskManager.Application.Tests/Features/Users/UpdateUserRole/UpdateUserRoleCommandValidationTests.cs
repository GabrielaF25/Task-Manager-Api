using FluentValidation.TestHelper;
using NUnit.Framework;
using TaskManager.Application.Features.Users.UpdateUserRole;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tests.Features.Users.UpdateUserRole;

[TestFixture]
public class UpdateUserRoleCommandValidationTests
{
    private UpdateUserRoleCommandValidation _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateUserRoleCommandValidation();
    }

    [Test]
    public async Task Should_Not_Have_Error_When_Role_Is_Valid()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            Role = UserRole.Admin
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Test]
    public async Task Should_Have_Error_When_Role_Is_Invalid()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            Role = (UserRole)999
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("Invalid role.");
    }
}