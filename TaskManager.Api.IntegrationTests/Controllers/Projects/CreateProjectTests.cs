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

public class CreateProjectTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Create_Project()
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

        var project = new CreateProjectRequest()
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        // Act

        var responseRequest = await Client.PostAsJsonAsync("api/projects", project);

        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var response = await responseRequest.Content.ReadFromJsonAsync<ProjectDto>();

        // Assert 

        Assert.That(response, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(response.Name, Is.EqualTo(project.Name));
            Assert.That(response.Description, Is.EqualTo(project.Description));
        });

        // Verify in database

        using var scope =  Factory.Services.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var projectFromDb = await dbContext.Projects.FirstOrDefaultAsync();

        Assert.That(projectFromDb, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(projectFromDb.Name, Is.EqualTo(project.Name));
            Assert.That(projectFromDb.Description, Is.EqualTo(project.Description));
        });

    }
}
