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


        var createRequest1 = new CreateTodoRequest()
        {
            Title = "Test Todo1",
            Description = "Test Description1",
            ProjectId = contentProject.Id
        };
        var createRequest2 = new CreateTodoRequest()
        {
            Title = "Test Todo2",
            Description = "Test Description2",
            ProjectId = contentProject.Id
        };

        var response1 = await Client.PostAsJsonAsync("/api/todos", createRequest1);
        var response2 = await Client.PostAsJsonAsync("/api/todos", createRequest2);

        Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var contentTodo1 = await response1.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.That(contentTodo1, Is.Not.Null);

        // Act
        var getTodo = await Client.GetAsync($"/api/todos/{contentTodo1.Id}");

        Assert.That(getTodo.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var contentGetTodo = await getTodo.Content.ReadFromJsonAsync<TodoResponse>();

        // Assert

        Assert.That(contentGetTodo, Is.Not.Null);
        Assert.That(contentGetTodo.Id, Is.EqualTo(contentTodo1.Id));
    }

    [Test]
    public async Task When_Get_TodoById_Should_Return_NotFound()
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

        var getTodo = await Client.GetAsync($"/api/todos/{Guid.NewGuid()}");

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
