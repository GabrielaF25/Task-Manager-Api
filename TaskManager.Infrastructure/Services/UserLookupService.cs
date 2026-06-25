using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Infrastructure.Services;

public class UserLookupService : IUserLookupService
{
    private readonly TaskManagerDbContext _dbContext;

    public UserLookupService(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }
    public async Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AnyAsync(u => u.UserName == userName, cancellationToken);
    }
}
