using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Authentication.Logout;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public LogoutUserCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.Request.RefreshToken, cancellationToken);

        if(refreshToken == null || !refreshToken.IsActive)
        {
            return Result.Failed(["Invalid refresh token."], StatusType.Forbidden);
        }

        refreshToken.Revoke();

        return Result.Success();
    }
}
