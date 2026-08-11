using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.ProjectTests;

public abstract class ProjectBaseTest
{
    protected SqliteConnection _connection = null!;
    protected TaskManagerDbContext _dbContext = null!;
    protected ProjectRepository _projectRepository = null!;

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

        _projectRepository = new ProjectRepository(_dbContext);

    }

    [TearDown]

    public void TearDown()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
