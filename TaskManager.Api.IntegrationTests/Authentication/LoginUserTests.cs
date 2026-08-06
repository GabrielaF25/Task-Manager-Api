using System.Net;
using System.Net.Http.Json;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Users.CreateUser;

namespace TaskManager.Api.IntegrationTests.Authentication;

public class LoginUserTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Return_LoginResponse()
    {
        // Arrange
        var registerRequest = new CreateUserRequest
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123"
        };

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        Assert.That(
            registerResponse.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var loginRequest = new UserCredentials()
        {
            Email = "gabriela@test.com",
            Password = "Password123"
        };

        // Act
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var response = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert

        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        Assert.That(response, Is.Not.Null);
        Assert.That(response.AccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(response.RefreshToken, Is.Not.Null.And.Not.Empty);
    }
}
