using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Api.IntegrationTests.Authentication;

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

        var responseRequest = await Client.GetAsync($"/api/auth/{1}");

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var contentResponse = await responseRequest.Content.ReadFromJsonAsync<UserResponse>(jsonOptions);
        // Assert

        Assert.That(contentResponse, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentResponse.Id, Is.EqualTo(1));
            Assert.That(contentResponse.UserName, Is.EqualTo(registeredUser.UserName));
            Assert.That(contentResponse.UserRole, Is.EqualTo(registeredUser.UserRole));
        });

    }
}
