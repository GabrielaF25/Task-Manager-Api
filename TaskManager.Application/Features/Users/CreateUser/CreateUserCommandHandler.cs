using AutoMapper;
using FluentValidation;
using MediatR;
using TaskManager.Application.Abstractions.Authetication;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasherService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserCommand userToCreate, CancellationToken cancellationToken)
    {
        var normalizedEmail = userToCreate.UserToCreate.Email.Trim().ToLowerInvariant();

       var normalizedUser = new CreateUserRequest() { UserName = userToCreate.UserToCreate.UserName, Email = normalizedEmail, Password = userToCreate.UserToCreate.Password };

        var user = User.Register(normalizedUser.Email, normalizedUser.UserName);

        var hash = _passwordHasherService.HashPassword(user, userToCreate.UserToCreate.Password);

        user.SetPasswordHash(hash);

        var userCreated = await _userRepository.CreateUserAsync(user, cancellationToken);

        var mappedUsers = _mapper.Map<UserResponse>(userCreated);
        return Result<UserResponse>.Success(mappedUsers);
    }
}
