using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.IntegrationTests.Controllers.Projects;

public class Get_ProjectByIdTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Return_Project_ById()
    {
        // Arrange

        var registerRequest = new CreateUserRequest
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123",
            Role = UserRole.User
        };

        var registerResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            registerRequest);

        Assert.That(
            registerResponse.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        var registeredUser = await registerResponse.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(registeredUser, Is.Not.Null);

        TestUserContext.Role = registeredUser.UserRole;
        TestUserContext.UserId = registeredUser.Id;

        var project1 = new CreateProjectRequest()
        {
            Name = "Test Project1",
            Description = "Test Description1"
        };

        var project2 = new CreateProjectRequest()
        {
            Name = "Test Project2",
            Description = "Test Description2"
        };

        var responseRequest1 = await Client.PostAsJsonAsync("api/projects", project1);
        var responseRequest2 = await Client.PostAsJsonAsync("api/projects", project2);

        Assert.That(responseRequest1.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        Assert.That(responseRequest2.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var content = await responseRequest1.Content.ReadFromJsonAsync<ProjectDto>(jsonOptions);

        Assert.That(content, Is.Not.Null);

        // Act 

        var response = await Client.GetAsync($"/api/projects/{content.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var contentResponse = await response.Content.ReadFromJsonAsync<ProjectDto>();

        // Assert
        Assert.That(contentResponse, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentResponse.Name, Is.EqualTo(project1.Name));
            Assert.That(contentResponse.Description, Is.EqualTo(project1.Description));
        });
    }

    [Test]
    public async Task When_Project_ById_Should_Return_NotFound()
    {
        // Arrange

        TestUserContext.Role = UserRole.User; // change the role for authenticate

        var registerRequest = new CreateUserRequest
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123",
            Role = UserRole.User
        };

        var registerResponse = await Client.PostAsJsonAsync(
            "/api/auth/register",
            registerRequest);

        Assert.That(
            registerResponse.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        var registeredUser = await registerResponse.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(registeredUser, Is.Not.Null);

        TestUserContext.Role = registeredUser.UserRole;
        TestUserContext.UserId = registeredUser.Id;

        // Act 

        var response = await Client.GetAsync($"/api/projects/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert

        Assert.That(content, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(content.Status, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(content.Title, Is.EqualTo("Resource not found"));
            Assert.That(content.Detail, Is.Not.Null);
            Assert.That(content.Instance, Is.Not.Null);
        });
    }
}
