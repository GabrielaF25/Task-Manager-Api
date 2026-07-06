using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Authetication;

public interface IJwtTokenGenerator
{
    string GenerateJwt(User user);
    string GenerateRefreshToken();
    DateTimeOffset GerRefreshTokenExperation();
}