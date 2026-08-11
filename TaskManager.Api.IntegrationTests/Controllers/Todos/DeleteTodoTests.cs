using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Todos.CreateTodo;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Api.IntegrationTests.Controllers.Todos;

public class DeleteTodoTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Delete_Todo()
    {
        // Arrange

        TestUserContext.Role = UserRole.User; // change the role for authenticate

        // register user

        var registerRequest = new CreateUserRequest
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123"
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

        // create project

        var project = new CreateProjectRequest()
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var responseRequest = await Client.PostAsJsonAsync("api/projects", project);

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        TestUserContext.Role = UserRole.Admin; // change the role for authenticate

        var createRequest = new CreateTodoRequest()
        {
            Title = "Test Todo",
            Description = "Test Description",
            ProjectId = 1
        };

        var response = await Client.PostAsJsonAsync("/api/todos", createRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        // Act

        var responseDeleted = await Client.DeleteAsync($"/api/todos/{1}");

        // Assert - HTTP

        Assert.That(responseDeleted.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        //Assert - Database

        using var scope = Factory.Services.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var todoFromDb = await dbContext.TodoItems.FirstOrDefaultAsync();

        Assert.That(todoFromDb, Is.Null);
    }

    [Test]
    public async Task When_Delete_Todo_Should_Return_NotFouns()
    {
        // Arrange

        TestUserContext.Role = UserRole.User; // change the role for authenticate

        // register user

        var registerRequest = new CreateUserRequest
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123"
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

        // create project

        var project = new CreateProjectRequest()
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var responseRequest = await Client.PostAsJsonAsync("api/projects", project);

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        TestUserContext.Role = UserRole.Admin; // change the role for authenticate

        // Act

        var responseDeleted = await Client.DeleteAsync($"/api/todos/{1}");

        Assert.That(responseDeleted.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var responseContent = await responseDeleted.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert

        Assert.That(responseContent, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(responseContent.Status, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(responseContent.Title, Is.EqualTo("Resource not found"));
            Assert.That(responseContent.Detail, Is.Not.Null);
            Assert.That(responseContent.Instance, Is.Not.Null);
        });
    }
}
