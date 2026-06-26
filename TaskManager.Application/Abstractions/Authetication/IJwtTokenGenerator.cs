using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateJwt(User user);
}