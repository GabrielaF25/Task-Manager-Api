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
