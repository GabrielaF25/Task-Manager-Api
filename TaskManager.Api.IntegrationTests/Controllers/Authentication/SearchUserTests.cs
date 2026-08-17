using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Api.IntegrationTests.Controllers.Authentication;

public class SearchUserTests : IntegrationTestBase
{
    [Test]

    public async Task Should_Return_Requested_User()
    {
        // Arrange
        var request = new CreateUserRequest()
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123"
        };
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var registeredUser = await response.Content.ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(registeredUser, Is.Not.Null);
        // Act

        var responseRequest = await Client.GetAsync($"/api/auth/{registeredUser.Id}");

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var contentResponse = await responseRequest.Content.ReadFromJsonAsync<UserResponse>(jsonOptions);
        // Assert

        Assert.That(contentResponse, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentResponse.Id, Is.EqualTo(registeredUser.Id));
            Assert.That(contentResponse.UserName, Is.EqualTo(registeredUser.UserName));
            Assert.That(contentResponse.UserRole, Is.EqualTo(registeredUser.UserRole));
        });

    }

    [Test]
    public async Task Should_Return_NotFound_When_Requested_User()
    {
        // Arrange
        
        // Act

        var responseRequest = await Client.GetAsync($"/api/auth/{Guid.NewGuid()}");

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var contentResponse = await responseRequest.Content.ReadFromJsonAsync<ProblemDetails>();
        // Assert

        Assert.That(contentResponse, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentResponse, Is.Not.Null);
            Assert.That(contentResponse.Title, Is.EqualTo("Resource not found"));
            Assert.That(contentResponse, Is.Not.Null);
        });
    }
}
