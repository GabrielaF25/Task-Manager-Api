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
using TaskManager.Application.Features.Todos.Dtos;
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

        var userAdmin = new CreateUserRequest
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123",
            Role = UserRole.User
        };

        var registerRequestUserAdmin = await Client.PostAsJsonAsync(
            "/api/auth/register",
            userAdmin);

        Assert.That(
            registerRequestUserAdmin.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));


        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        var content = await registerRequestUserAdmin.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(content, Is.Not.Null);

        TestUserContext.Role = content.UserRole;
        TestUserContext.UserId = content.Id;

        // create project

        var project = new CreateProjectRequest()
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var responseRequest = await Client.PostAsJsonAsync("api/projects", project);
        var contentProject = await responseRequest.Content.ReadFromJsonAsync<ProjectDto>();

        Assert.That(contentProject, Is.Not.Null);

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var user = new CreateUserRequest
        {
            UserName = "GabrielaTest",
            Email = "gabriela@testuser.com",
            Password = "Password123",
            Role = UserRole.Admin
        };

        var registerRequestUser = await Client.PostAsJsonAsync(
            "/api/auth/register",
            user);

        Assert.That(
            registerRequestUser.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var contentUser = await registerRequestUser.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(contentUser, Is.Not.Null);

        TestUserContext.Role = contentUser.UserRole;
        TestUserContext.UserId = contentUser.Id;

        var createRequest = new CreateTodoRequest()
        {
            Title = "Test Todo",
            Description = "Test Description",
            ProjectId = contentProject.Id
        };

        var response = await Client.PostAsJsonAsync("/api/todos", createRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var contentResponse = await response.Content.ReadFromJsonAsync<TodoResponse>();

        Assert.That(contentResponse, Is.Not.Null);

        // Act

        var responseDeleted = await Client.DeleteAsync($"/api/todos/{contentResponse.Id}");

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

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        var user = new CreateUserRequest
        {
            UserName = "GabrielaTest",
            Email = "gabriela@testuser.com",
            Password = "Password123",
            Role = UserRole.Admin
        };

        var registerRequestUser = await Client.PostAsJsonAsync(
            "/api/auth/register",
            user);

        Assert.That(
            registerRequestUser.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var contentUser = await registerRequestUser.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(contentUser, Is.Not.Null);

        TestUserContext.Role = contentUser.UserRole;
        TestUserContext.UserId = contentUser.Id;


        // Act

        var responseDeleted = await Client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");

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
