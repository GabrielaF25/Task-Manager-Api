using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Application.Features.Users.UpdateUserRole;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Api.IntegrationTests.Authentication;

public class ChangeUserRoleTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Change_UserRole()
    {
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        // Arrange - admin
        var registerAdmin = new CreateUserRequest
        {
            UserName = "Admin",
            Email = "admin@test.com",
            Password = "Password123",
            Role = UserRole.Admin
        };

        var registerAdminResponse =
            await Client.PostAsJsonAsync("/api/auth/register", registerAdmin);

        Assert.That(
            registerAdminResponse.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var registeredAdmin = await registerAdminResponse.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(registeredAdmin, Is.Not.Null);

        TestUserContext.UserId = registeredAdmin.Id;
        TestUserContext.Role = registeredAdmin.UserRole;

        // Arrange - normal user
        var registerUser = new CreateUserRequest
        {
            UserName = "User",
            Email = "user@test.com",
            Password = "Password123",
            Role = UserRole.User
        };

        var registerUserResponse =
            await Client.PostAsJsonAsync("/api/auth/register", registerUser);

        Assert.That(
            registerUserResponse.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var registeredUser = await registerUserResponse.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(registeredUser, Is.Not.Null);

        var updateUserRequest = new UpdateUserRequest
        {
            Id = registeredUser.Id,
            Role = UserRole.Admin
        };

        // Act
        var responseRequest = await Client.PatchAsJsonAsync(
            "/api/auth/role",
            updateUserRequest);

        // Assert - HTTP
        Assert.That(
            responseRequest.StatusCode,
            Is.EqualTo(HttpStatusCode.OK));

        var response = await responseRequest.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(response, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(updateUserRequest.Id));
            Assert.That(response.UserRole, Is.EqualTo(UserRole.Admin));
        });

        // Assert - Database
        using var scope = Factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<TaskManagerDbContext>();

        var userFromDb = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == updateUserRequest.Id);

        Assert.That(userFromDb, Is.Not.Null);
        Assert.That(userFromDb!.UserRole, Is.EqualTo(UserRole.Admin));
    }

    [Test]
    public async Task When_User_Not_Exist_Should_Return_ProblemDetails()
    {

        // Arrange


        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        // Arrange - admin
        var registerAdmin = new CreateUserRequest
        {
            UserName = "Admin",
            Email = "admin@test.com",
            Password = "Password123",
            Role = UserRole.Admin
        };

        var registerAdminResponse =
            await Client.PostAsJsonAsync("/api/auth/register", registerAdmin);

        Assert.That(
            registerAdminResponse.StatusCode,
            Is.EqualTo(HttpStatusCode.Created));

        var registeredAdmin = await registerAdminResponse.Content
            .ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(registeredAdmin, Is.Not.Null);

        TestUserContext.UserId = registeredAdmin.Id;
        TestUserContext.Role = registeredAdmin.UserRole;
        // Act

        var responseRequest = await Client.PatchAsJsonAsync("/api/auth/role", new UpdateUserRequest { Id = Guid.NewGuid(), Role = UserRole.Unknown});
        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));


        var response = await responseRequest.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert - HTTP
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(response.Title, Is.EqualTo("Resource not found"));
    }

}
