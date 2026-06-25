using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}