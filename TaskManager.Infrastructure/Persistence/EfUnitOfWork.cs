using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Domain.Common;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Infrastructure.Persistence;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly TaskManagerDbContext _dbContext;
    private readonly IPublisher _publisher;

    public EFUnitOfWork(TaskManagerDbContext dbContext, IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

   public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _dbContext.SaveChangesAsync(ct);
  
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        return new EfTransaction(transaction);
    }

    public async Task DispatchDomainEventAsync(CancellationToken ct)
    {
        var entities = _dbContext.ChangeTracker.Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity)
                        .ToList();

        var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

        entities.ForEach(e => e.ClearDomainEvent());

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, ct);
        }
    }
}
