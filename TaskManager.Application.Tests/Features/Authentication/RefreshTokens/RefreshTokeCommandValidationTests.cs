using FluentAssertions;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Authentication.RefreshTokens;

namespace TaskManager.Application.Tests.Features.Authentication.RefreshTokens;

[TestFixture]
public class RefreshTokenCommandValidationTests
{
    private RefreshTokenCommandValidation _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new RefreshTokenCommandValidation();
    }

    [Test]
    public void Validate_Should_Return_Error_When_RefreshToken_Is_Null()
    {
        // Arrange
        var command = new RefreshTokenCommand(new RefreshTokenRequest
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
    public void Validate_Should_Return_Error_When_RefreshToken_Is_Empty()
    {
        // Arrange
        var command = new RefreshTokenCommand(new RefreshTokenRequest
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
    public void Validate_Should_Return_Valid_When_RefreshToken_Is_Provided()
    {
        // Arrange
        var command = new RefreshTokenCommand(new RefreshTokenRequest
        {
            RefreshToken = "valid-refresh-token"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}