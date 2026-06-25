using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Authentication;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string passwordHash, string password);
}
