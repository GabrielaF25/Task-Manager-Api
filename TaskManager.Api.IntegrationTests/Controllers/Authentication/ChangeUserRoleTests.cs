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
        var registerRequest = new CreateUserRequest
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123"
        };

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
        Assert.That(
         registerResponse.StatusCode,
         Is.EqualTo(HttpStatusCode.Created));

        var updateUserRequest = new UpdateUserRequest()
        {
            Id = 1,
            Role = UserRole.Unknown

        };
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        // Act

        var responseRequest = await Client.PatchAsJsonAsync("/api/auth/role", updateUserRequest);
        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.OK));


        var response = await responseRequest.Content.ReadFromJsonAsync<UserResponse>(jsonOptions);

        // Assert - HTTP
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo(updateUserRequest.Id));
        Assert.That(response.UserRole, Is.EqualTo(UserRole.Unknown));

        using var scope = Factory.Services.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var user = await dbContext.Users.SingleOrDefaultAsync();

        // Assert - Database

        Assert.That(user, Is.Not.Null);
        Assert.That(user.UserRole, Is.EqualTo(UserRole.Unknown));
    }

    [Test]
    public async Task When_User_Not_Exist_Should_Return_ProblemDetails()
    {
     
        // Arrange

        // Act

        var responseRequest = await Client.PatchAsJsonAsync("/api/auth/role", new UpdateUserRequest { Id = 99, Role = UserRole.Unknown});
        Assert.That(responseRequest.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));


        var response = await responseRequest.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert - HTTP
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(response.Title, Is.EqualTo("Resource not found"));
    }

}
