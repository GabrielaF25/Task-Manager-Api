using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.Repository;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure.IntegrationTests.Services;

public class UserLookupTests
{
    protected SqliteConnection _connection = null!;
    protected TaskManagerDbContext _dbContext = null!;
    protected UserLookupService _userLookupService = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("Data Source =:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new TaskManagerDbContext(options);

        _dbContext.Database.EnsureCreated();

        _userLookupService = new UserLookupService(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
        _connection?.Dispose();
    }

    [Test]
    public async Task EmailExistsAsync_Shoul_Return_True()
    {
        // Arrange

        var user = User.Register("test@test.ro", "test user");
        user.SetPasswordHash("hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act

        var response = await _userLookupService.EmailExistsAsync("test@test.ro", CancellationToken.None);

        // Assert

        Assert.True(response);
    }

    [Test]
    public async Task EmailExistsAsync_Shoul_Return_False()
    {
        // Arrange


        // Act

        var response = await _userLookupService.EmailExistsAsync("test@test.ro", CancellationToken.None);

        // Assert

        Assert.False(response);
    }

    [Test]
    public async Task UserNameExistsAsync_Shoul_Return_True()
    {
        // Arrange

        var user = User.Register("test@test.ro", "test user");
        user.SetPasswordHash("hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act

        var response = await _userLookupService.UserNameExistsAsync("test user", CancellationToken.None);

        // Assert

        Assert.True(response);
    }

    [Test]
    public async Task UserNameExistsAsync_Shoul_Return_False()
    {
        // Arrange


        // Act

        var response = await _userLookupService.UserNameExistsAsync("test user", CancellationToken.None);

        // Assert

        Assert.False(response);
    }
}
