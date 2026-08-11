using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.UnitTests.Persistence;

public class EfTransactionTests
{
    private Mock<IDbContextTransaction> _transactionMock = null!;
    private EfTransaction _efTransaction = null!;

    [SetUp]
    public void SetUp()
    {
        _transactionMock = new Mock<IDbContextTransaction>();

        _efTransaction = new EfTransaction(
            _transactionMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _efTransaction.DisposeAsync();
    }

    [Test]
    public async Task CommitAsync_Should_Call_CommitAsync()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        await _efTransaction.CommitAsync(cancellationToken);

        // Assert
        _transactionMock.Verify(
            x => x.CommitAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task RollbackAsync_Should_Call_RollbackAsync()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        await _efTransaction.RollbackAsync(cancellationToken);

        // Assert
        _transactionMock.Verify(
            x => x.RollbackAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task DisposeAsync_Should_Call_DisposeAsync()
    {
        // Act
        await _efTransaction.DisposeAsync();

        // Assert
        _transactionMock.Verify(
            x => x.DisposeAsync(),
            Times.Once);
    }
}
