using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Authentication;

namespace TaskManager.Application.Features.Authentication.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingRefreshToken =
            await _refreshTokenRepository.GetByTokenAsync(request.Request.RefreshToken, cancellationToken);

        if(existingRefreshToken is null || !existingRefreshToken.IsActive)
        {
            return Result<RefreshTokenResponse>.Failed(["Invalid refresh token."], StatusType.Unauthorized);
        }

        var user = existingRefreshToken.User;

        existingRefreshToken.Revoke();

        var newAccessToken = _jwtTokenGenerator.GenerateJwt(user);

        var newRefreshToken = RefreshToken.Create(_jwtTokenGenerator.GenerateRefreshToken(),  _jwtTokenGenerator.GerRefreshTokenExperation());

        user.AddRefreshToken(newRefreshToken);

        return Result<RefreshTokenResponse>.Success(
          new RefreshTokenResponse
          {
              AccessToken = newAccessToken,
              RefreshToken = newRefreshToken.Token
          });

    }
}
