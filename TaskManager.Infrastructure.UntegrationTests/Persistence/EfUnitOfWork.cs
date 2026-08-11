using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Events;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.IntegrationTests.Persistence;

public class EfUnitOfWork
{
    private Mock<IPublisher> publisherMock = null!;
    private EFUnitOfWork unitOfWork = null!;
    private TaskManagerDbContext _dbContext = null!;
    private SqliteConnection _connection = null!;

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

        publisherMock = new Mock<IPublisher>();
        unitOfWork = new EFUnitOfWork(_dbContext, publisherMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    [Test]
    public async Task SaveChangesAsync_Should_Persist_Changes()
    {
        // Arrange

        var user = User.Register(
            "test@test.com",
            "Test");

        user.SetPasswordHash("hash");

        _dbContext.Users.Add(user);

        // Act

        var result = await unitOfWork.SaveChangesAsync(
            CancellationToken.None);

        // Assert
        var userFromDb = await _dbContext.Users
            .FirstOrDefaultAsync();

        Assert.That(result, Is.EqualTo(1));
        Assert.That(userFromDb, Is.Not.Null);
    }

    [Test]
    public async Task BeginTransactionAsync_Should_Return_Transaction()
    {
        // Arrange

        var publisherMock = new Mock<IPublisher>();

        var unitOfWork = new EFUnitOfWork(
            _dbContext,
            publisherMock.Object);

        // Act
        var transaction = await unitOfWork.BeginTransactionAsync(
            CancellationToken.None);

        // Assert
        Assert.That(transaction, Is.Not.Null);

        await transaction.DisposeAsync();
    }

    [Test]
    public async Task DispatchDomainEventAsync_Should_Publish_And_Clear_DomainEvents()
    {
        // Arrange

        var publisherMock = new Mock<IPublisher>();

        var user = User.Register(
            "test@test.ro",
            "Test");

        user.SetPasswordHash("hash");

        _dbContext.Users.Add(user);

        var unitOfWork = new EFUnitOfWork(
            _dbContext,
            publisherMock.Object);

        Assert.That(user.DomainEvents, Is.Not.Empty);

        // Act

        await unitOfWork.DispatchDomainEventAsync(
            CancellationToken.None);

        // Assert

        publisherMock.Verify(
      x => x.Publish(
          It.Is<INotification>(e => e is UserRegisteredEvent),
          It.IsAny<CancellationToken>()),
      Times.Once);

        Assert.That(user.DomainEvents, Is.Empty);
    }
}
