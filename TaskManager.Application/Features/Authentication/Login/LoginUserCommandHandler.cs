using MediatR;
using TaskManager.Application.Abstractions.Authetication;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Authentication.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(IPasswordHasherService passwordHasherService, IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _passwordHasherService = passwordHasherService;
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUseEmail(command.UserCredentials.Email.ToLowerInvariant().Trim(), cancellationToken);

        if (user == null)
        {
            var error = new List<string> { "Invalid credentials." };
            return Result<LoginResponse>.Failed(error, StatusType.Unauthorized);
        }

        var result = _passwordHasherService.VerifyPassword(user, user.PasswordHash, command.UserCredentials.Password);

        if (!result)
        {
            var error = new List<string> { "Invalid credentials." };
            return Result<LoginResponse>.Failed(error, StatusType.Unauthorized);
        }
        var accessToken = _jwtTokenGenerator.GenerateJwt(user);

        var refreshToken = RefreshToken.Create(_jwtTokenGenerator.GenerateRefreshToken(),  _jwtTokenGenerator.GerRefreshTokenExperation());

        user.AddRefreshToken(refreshToken);

        return Result<LoginResponse>.Success(new LoginResponse
        {
            UserName = user.UserName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        });
    }
}