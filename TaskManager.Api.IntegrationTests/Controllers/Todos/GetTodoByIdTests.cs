using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

namespace TaskManager.Api.IntegrationTests.Controllers.Todos;

public class GetTodoByIdTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Get_TodoById()
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

        var createRequest1 = new CreateTodoRequest()
        {
            Title = "Test Todo1",
            Description = "Test Description1",
            ProjectId = 1
        };
        var createRequest2 = new CreateTodoRequest()
        {
            Title = "Test Todo2",
            Description = "Test Description2",
            ProjectId = 1
        };

        var response1 = await Client.PostAsJsonAsync("/api/todos", createRequest1);
        var response2 = await Client.PostAsJsonAsync("/api/todos", createRequest2);

        Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        // Act

        var getTodo = await Client.GetAsync($"/api/todos/{1}");

        Assert.That(getTodo.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var contentGetTodo = await getTodo.Content.ReadFromJsonAsync<TodoResponse>();

        // Assert

        Assert.That(contentGetTodo, Is.Not.Null);
        Assert.That(contentGetTodo.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task When_Get_TodoById_Should_Return_NotFound()
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

        var getTodo = await Client.GetAsync($"/api/todos/{1}");

        Assert.That(getTodo.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var contentProblem = await getTodo.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert

        Assert.That(contentProblem, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentProblem.Status, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(contentProblem.Detail, Is.Not.Null);
            Assert.That(contentProblem.Title, Is.EqualTo("Resource not found"));
        });

    }
}
