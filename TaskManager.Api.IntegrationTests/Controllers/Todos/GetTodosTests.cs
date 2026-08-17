using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Todos.CreateTodo;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.IntegrationTests.Controllers.Todos;

public class GetTodosTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Get_PaginatedTodos()
    {

        // Arrange

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

        // Act

        var responseTodos = await Client.GetAsync("/api/todos");

        Assert.That(responseTodos.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var contentResponseTodos = await responseTodos.Content.ReadFromJsonAsync<PaginationResult<TodoResponse>>();

        // Assert

        Assert.That(contentResponseTodos, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentResponseTodos.TotalCount, Is.EqualTo(2));
            Assert.That(contentResponseTodos.HasPreviousPage, Is.False);
            Assert.That(contentResponseTodos.HasNextPage, Is.False);
            Assert.That(contentResponseTodos.Items, Is.Not.Null);
            Assert.That(contentResponseTodos.TotalPages, Is.EqualTo(1));
            Assert.That(contentResponseTodos.PageSize, Is.EqualTo(10));
        });
    }
}
