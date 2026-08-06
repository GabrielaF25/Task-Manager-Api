using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Api.IntegrationTests.Common;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Api.IntegrationTests.Authentication;

public class RegisterUserTests : IntegrationTestBase
{
    [Test]
    public async Task Register_Should_Create_User()
    {
        // Arrange
        var request = new CreateUserRequest()
        {
            UserName = "Gabriela",
            Email = "gabriela@test.com",
            Password = "Password123"
        };

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        jsonOptions.Converters.Add(new JsonStringEnumConverter());
        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert -Http response
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>(jsonOptions);

        Assert.That(userResponse, Is.Not.Null);
        Assert.That(userResponse.UserName, Is.EqualTo(request.UserName));
        Assert.That(userResponse.Email, Is.EqualTo(request.Email));

        //Assert - database

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        var userFromDatabase = await dbContext.Users
            .SingleOrDefaultAsync(user => user.Email == request.Email);

        Assert.That(userFromDatabase, Is.Not.Null);
        Assert.That(userFromDatabase!.UserName, Is.EqualTo(request.UserName));
        Assert.That(userFromDatabase.Email, Is.EqualTo(request.Email));
    }
}       
