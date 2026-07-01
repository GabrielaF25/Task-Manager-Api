using Microsoft.AspNetCore.Identity;
using TaskManager.Application.Abstractions.Authetication;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Services.Authentication;

public class PasswordHasherService : IPasswordHasherService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    public PasswordHasherService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(User user,string password)
    {
        return _passwordHasher.HashPassword(user,password);
    }

    public bool VerifyPassword(User user,string passwordHash, string password)
    {
       
        var result = _passwordHasher.VerifyHashedPassword(user,passwordHash, password);

        return result == PasswordVerificationResult.Success;
    }
}
