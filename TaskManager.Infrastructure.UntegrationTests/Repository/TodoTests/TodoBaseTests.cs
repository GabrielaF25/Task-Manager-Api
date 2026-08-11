using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Bson;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.TodoTests;

public abstract class TodoBaseTests
{
    protected SqliteConnection _connection = null!;
    protected TaskManagerDbContext _dbContext = null!;
    protected TodoRepository _todoRepository = null!;

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

        _todoRepository = new TodoRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
        _connection?.Dispose();
    }
}
