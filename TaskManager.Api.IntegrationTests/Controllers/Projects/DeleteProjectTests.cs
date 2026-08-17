using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Api.IntegrationTests.Controllers.Projects;

public class DeleteProjectTests : IntegrationTestBase
{
    [Test]

    public async Task Should_Delete_Project()
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

        var project = new CreateProjectRequest()
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var responseRequest = await Client.PostAsJsonAsync("api/projects", project);

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var content = await responseRequest.Content.ReadFromJsonAsync<UserResponse>();
        Assert.That(content, Is.Not.Null);


        // Act

        var deleteRequest = await Client.DeleteAsync($"/api/projects/{content.Id}");

        // Assert - HTTP

        Assert.That(deleteRequest.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Assert - Database

        using var scope = Factory.Services.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var deletedProjectFromDb = await dbContext.Projects.FirstOrDefaultAsync();

        Assert.That(deletedProjectFromDb, Is.Null);
    }

    [Test]
    public async Task When_Delete_Should_Return_NotFound()
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

        var registerResponse = await Client!.PostAsJsonAsync(
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

        var deleteRequest = await Client.DeleteAsync($"/api/projects/{Guid.NewGuid()}");

        Assert.That(deleteRequest.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var content = await deleteRequest.Content.ReadFromJsonAsync<ProblemDetails>();

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
