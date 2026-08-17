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

public class CreateTodoTests: IntegrationTestBase
{
    [Test]
    public async Task Should_Create_Todo()
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

        // Act

        var response = await Client.PostAsJsonAsync("/api/todos", createRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var contentResponse  = await response.Content.ReadFromJsonAsync<TodoResponse>();

        // Assert - HTTP
        Assert.That(contentResponse, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentResponse.Title, Is.EqualTo(createRequest.Title));
            Assert.That(contentResponse.Description, Is.EqualTo(createRequest.Description));
        });

        // Assert - Database

        using var scope = Factory.Services.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var todosFromDb = await dbContext.TodoItems.FirstOrDefaultAsync();

        Assert.That(todosFromDb, Is.Not.Null);

        Assert.Multiple(() => 
        {
            Assert.That(todosFromDb.Title, Is.EqualTo(createRequest.Title));
            Assert.That(todosFromDb.Description, Is.EqualTo(createRequest.Description));
        });
    }
}
