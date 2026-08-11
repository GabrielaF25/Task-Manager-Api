using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Users.CreateUser;

namespace TaskManager.Api.IntegrationTests.Controllers.Authentication;

public class RefreshTokenTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Return_Refresh_Token()
    {
        // Arrange

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

        var responseContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(responseContent, Is.Not.Null);

        var tokenRequest = new RefreshTokenRequest()
        {
            RefreshToken = responseContent.RefreshToken
        };

        // Act
        var responseRefreshToken = await Client.PostAsJsonAsync("/api/auth/refresh", tokenRequest);
        var response = await responseRefreshToken.Content.ReadFromJsonAsync<RefreshTokenResponse>();

        // Assert

        Assert.That(responseRefreshToken, Is.Not.Null);
        Assert.That(responseRefreshToken.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response, Is.Not.Null); 
    }

    [Test]
    public async Task When_RefreshToken_Expired_Should_Return_Failed()
    {
        // Arrange

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

        var responseContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(responseContent, Is.Not.Null);

        var tokenRequest = new RefreshTokenRequest()
        {
            RefreshToken = responseContent.RefreshToken
        };

        var responseRefreshToken = await Client.PostAsJsonAsync("/api/auth/refresh", tokenRequest);
        var response = await responseRefreshToken.Content.ReadFromJsonAsync<RefreshTokenResponse>();


        // Request Again With the same Refresh Token

        var requestAgainToken = await Client.PostAsJsonAsync("/api/auth/refresh", tokenRequest);

        Assert.That(requestAgainToken.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var content = await requestAgainToken.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert

        Assert.That(content, Is.Not.Null);
        Assert.That(content.Status, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(content.Title, Is.EqualTo("Unauthorized"));
    }
}
