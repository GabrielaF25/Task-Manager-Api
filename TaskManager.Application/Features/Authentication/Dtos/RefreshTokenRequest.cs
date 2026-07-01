using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Authentication.Dtos;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
