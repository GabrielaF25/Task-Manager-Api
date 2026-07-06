using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;

namespace TaskManager.Application.Features.Authentication.RefreshTokens;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result<RefreshTokenResponse>>;
