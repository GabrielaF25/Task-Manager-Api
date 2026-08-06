using MediatR;
using Moq;
using NUnit.Framework;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Behaviors;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Tests.Behaviors;

[TestFixture]
public class UnitOfWorkBehaviorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<ITransaction> _transactionMock = null!;
    private UnitOfWorkBehavior<TestRequest, Result> _behavior = null!;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _transactionMock = new Mock<ITransaction>();

        _unitOfWorkMock
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);

        _behavior = new UnitOfWorkBehavior<TestRequest, Result>(
            _unitOfWorkMock.Object);
    }

    [Test]
    public async Task Handle_Should_Commit_And_Dispatch_DomainEvents_When_Response_Is_Success()
    {
        var request = new TestRequest();

        RequestHandlerDelegate<Result> next = ct =>
            Task.FromResult(Result.Success());

        var result = await _behavior.Handle(request, next, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.DispatchDomainEventAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionMock.Verify(x =>
            x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Rollback_When_Response_Is_Failure()
    {
        var request = new TestRequest();

        RequestHandlerDelegate<Result> next = ct =>
            Task.FromResult(Result.Failed(
                ["Something went wrong"],
                StatusType.ValidationError));

        var result = await _behavior.Handle(request, next, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);

        _transactionMock.Verify(x =>
            x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _transactionMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(x =>
            x.DispatchDomainEventAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public void Handle_Should_Rollback_And_Throw_When_Next_Throws_Exception()
    {
        var request = new TestRequest();

        RequestHandlerDelegate<Result> next = ct =>
            throw new InvalidOperationException("Test exception");

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _behavior.Handle(request, next, CancellationToken.None));

        _transactionMock.Verify(x =>
            x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _transactionMock.Verify(x =>
            x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    public sealed class TestRequest : IRequest<Result>
    {
    }
}