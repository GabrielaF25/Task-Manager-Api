using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Services.Authentication;

namespace TaskManager.Infrastructure.UnitTests.Services.Authentication;

public class JwtTokenGeneratorTests
{
    private JwtSettings _jwtSettings = null!;
    private JwtTokenGenerator _jwtTokenGenerator = null!;

    [SetUp]
    public void SetUp()
    {
        _jwtSettings = new JwtSettings
        {
            Key = "ThisIsAVeryLongSecretKeyForTesting123456789",
            Issuer = "TaskManager",
            Audience = "TaskManagerUsers",
            ExpirationInMinute = 30,
            RefreshTokenExpiryMinutes = 60
        };

        var options = Options.Create(_jwtSettings);

        _jwtTokenGenerator = new JwtTokenGenerator(options);
    }

    [Test]
    public void GenerateRefreshToken_Should_Return_Different_Tokens()
    {
        // Act
        var token1 = _jwtTokenGenerator.GenerateRefreshToken();
        var token2 = _jwtTokenGenerator.GenerateRefreshToken();

        // Assert
        Assert.That(token1, Is.Not.EqualTo(token2));
    }

    [Test]
    public void GetRefreshTokenExperation_Should_Return_Configured_Expiration()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var expiration = _jwtTokenGenerator.GetRefreshTokenExpiration();

        var after = DateTimeOffset.UtcNow;

        // Assert
        var minimumExpected =
            before.AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes);

        var maximumExpected =
            after.AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes);

        Assert.That(
            expiration,
            Is.InRange(minimumExpected, maximumExpected));
    }
}
