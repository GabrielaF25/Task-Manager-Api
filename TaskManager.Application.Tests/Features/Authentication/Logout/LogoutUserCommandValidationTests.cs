using FluentAssertions;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Authentication.Logout;

namespace TaskManager.Application.Tests.Features.Authentication.Logout;

[TestFixture]
public class LogoutUserCommandValidationTests
{
    private LogoutUserCommandValidation _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new LogoutUserCommandValidation();
    }

    [Test]
    public void Validate_Should_Return_Error_When_RefreshToken_Is_Empty()
    {
        // Arrange
        var command = new LogoutUserCommand(new RefreshTokenRequest
        {
            RefreshToken = string.Empty
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.PropertyName == "Request.RefreshToken");
    }

    [Test]
    public void Validate_Should_Return_Error_When_RefreshToken_Is_Null()
    {
        // Arrange
        var command = new LogoutUserCommand(new RefreshTokenRequest
        {
            RefreshToken = null!
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.PropertyName == "Request.RefreshToken");
    }

    [Test]
    public void Validate_Should_Return_Valid_When_RefreshToken_Is_Provided()
    {
        // Arrange
        var command = new LogoutUserCommand(new RefreshTokenRequest
        {
            RefreshToken = "refresh-token"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}