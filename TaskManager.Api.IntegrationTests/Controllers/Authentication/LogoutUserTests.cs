using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Api.IntegrationTests.Controllers.Authentication;

public class LogoutUserTests : IntegrationTestBase
{
    [Test]

    public async Task Should_Revoke_Token()
    {
        //Arrange
        var createUser = new CreateUserRequest()
        {
            UserName = "Test",
            Email = "test@test.ro",
            Password = "Tesssst12!"

        };

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", createUser);

        Assert.That(registerResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var loginRequest = new UserCredentials()
        {
            Email = "test@test.ro",
            Password = "Tesssst12!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(loginContent, Is.Not.Null);

        var token = new RefreshTokenRequest()
        {
            RefreshToken = loginContent.RefreshToken
        };

        // Act

        var logoutRequest = await Client.PostAsJsonAsync("/api/auth/logout", token);

        // Assert - HTTP
        
        Assert.That(logoutRequest, Is.Not.Null);
        Assert.That(logoutRequest.StatusCode,Is.EqualTo(HttpStatusCode.NoContent));

        // Assert - Database

        using var scope = Factory.Services.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var refreshToken = await dbContext.RefreshTokens
       .FirstOrDefaultAsync(t => t.Token == loginContent.RefreshToken);

        Assert.That(refreshToken, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(refreshToken!.IsRevoked, Is.True);
            Assert.That(refreshToken.IsActive, Is.False);
            Assert.That(refreshToken.RevokedAt, Is.Not.Null);
        });

    }

    [Test]

    public async Task When_Token_Invalid_Should_Return_Forbidden()
    {
        //Arrange

        // Act

        var logoutRequest = await Client.PostAsJsonAsync("/api/auth/logout", new RefreshTokenRequest() { RefreshToken = "Token Invalid"});

        Assert.That(logoutRequest.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var content = await logoutRequest.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert

        Assert.That(content, Is.Not.Null);
        Assert.That(content.Status, Is.EqualTo(StatusCodes.Status403Forbidden));
        Assert.That(content.Title, Is.EqualTo("Forbidden"));


    }
}
