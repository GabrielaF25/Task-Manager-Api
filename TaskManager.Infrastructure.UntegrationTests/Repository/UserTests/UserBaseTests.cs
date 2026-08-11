using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.UserTests;

public class UserBaseTests
{
    protected SqliteConnection _connection = null!;
    protected TaskManagerDbContext _dbContext = null!;
    protected UserRepository _userRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new TaskManagerDbContext(options);

        _dbContext.Database.EnsureCreated();

        _userRepository = new UserRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
