using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Infrastructure.Persistence;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly TaskManagerDbContext _dbContext;

    public EFUnitOfWork(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
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
}
