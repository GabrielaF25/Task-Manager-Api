using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Repository.TodoTests;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.RefreshTokenTests;

public class RefreshTokenTests : RefreshTokenBaseTests
{
    [Test]
    public async Task AddAsync_Should_Add_RefreshToken()
    {

        // Arrange

        var user = User.Register("test@test.ro", "Test");

        user.SetPasswordHash("Hash");


        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var token = RefreshToken.Create("Refresh Token", DateTimeOffset.Now.AddHours(1));
        user.RefreshTokens.Add(token);


        // Act

        await _refreshRepository.AddAsync(token, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert

        var refreshTokenFromDb = await _dbContext.RefreshTokens.FirstOrDefaultAsync();

        Assert.That(refreshTokenFromDb, Is.Not.Null);
        Assert.That(refreshTokenFromDb.Token, Is.EqualTo(token.Token));

    }

    [Test]
    public async Task GetByToken_Should_Get_RefreshToken()
    {

        // Arrange

        var user = User.Register("test@test.ro", "Test");

        user.SetPasswordHash("Hash");


        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var token = RefreshToken.Create("Refresh Token", DateTimeOffset.Now.AddHours(1));
        user.RefreshTokens.Add(token);


        await _dbContext.RefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();

        // Act

        var refreshTokenReturned = await _refreshRepository.GetByTokenAsync(token.Token, CancellationToken.None);

        // Assert


        Assert.That(refreshTokenReturned, Is.Not.Null);
        Assert.That(refreshTokenReturned.Token, Is.EqualTo(token.Token));

    }
}
