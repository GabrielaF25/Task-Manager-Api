using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Authetication;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string passwordHash, string password);
}
