using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;

namespace TaskManager.Application.Features.Authentication.Login;

public record LoginUserCommand(UserCredentials UserCredentials) :IRequest<Result<LoginResponse>>;
