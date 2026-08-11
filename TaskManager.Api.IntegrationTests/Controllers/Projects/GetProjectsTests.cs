using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.IntegrationTests.Controllers.Projects;

public class GetProjectsTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Get_PaginatedProjects()
    {
        // Arrange

        TestUserContext.Role = UserRole.User; // change the role for authenticate

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

        // Act

        var response = await Client.GetAsync("/api/projects");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));   
        
        var contentResponse = await response.Content.ReadFromJsonAsync<PaginationResult<ProjectDto>>();

        // Assert - HTTP

        Assert.That(contentResponse, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(contentResponse.TotalCount, Is.EqualTo(2));
            Assert.That(contentResponse.HasPreviousPage, Is.False);
            Assert.That(contentResponse.HasNextPage, Is.False);
            Assert.That(contentResponse.Items, Is.Not.Null);
            Assert.That(contentResponse.TotalPages, Is.EqualTo(1));
            Assert.That(contentResponse.PageSize, Is.EqualTo(10));
        });
    }
}
