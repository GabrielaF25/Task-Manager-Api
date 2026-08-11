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

public class UpdateTodoCompleteTests : IntegrationTestBase
{

    [Test]

    public async Task Should_Change_Complete_True_Todo()
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

        var changeCompleteTodo = await Client.PatchAsJsonAsync($"/api/todos/{1}/complete", "1");

        Assert.That(changeCompleteTodo.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var contentChangeComplete = await changeCompleteTodo.Content.ReadFromJsonAsync<TodoResponse>();

        // Assert - HTTP

        Assert.That(contentChangeComplete, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentChangeComplete.IsCompleted, Is.True);
            Assert.That(contentChangeComplete.Title, Is.EqualTo(createRequest.Title));
        });

        // Assert - Database

        using var scope = Factory.Services.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var completedTodo = await dbContext.TodoItems.FirstOrDefaultAsync();

        Assert.That(completedTodo, Is.Not.Null);
        Assert.That(completedTodo.IsCompleted, Is.True);
    }


    [Test]

    public async Task When_Change_Complete_True_Todo_Return_NotFound()
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

        var changeCompleteTodo = await Client.PatchAsJsonAsync($"/api/todos/{1}/complete", "1");

        Assert.That(changeCompleteTodo.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var contentProblem = await changeCompleteTodo.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert - HTTP

        Assert.That(contentProblem, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentProblem.Status, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(contentProblem.Title, Is.EqualTo("Resource not found"));
            Assert.That(contentProblem.Detail, Is.Not.Null);
            Assert.That(contentProblem.Instance, Is.Not.Null);
        });
    }
}
