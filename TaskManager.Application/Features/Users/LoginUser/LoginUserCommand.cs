using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Users.LoginUser;

public record LoginUserCommand(UserCredentials UserCredentials) :IRequest<Result<LoginResponse>>;
