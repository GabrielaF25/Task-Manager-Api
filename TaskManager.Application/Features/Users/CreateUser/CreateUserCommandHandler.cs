using AutoMapper;
using FluentValidation;
using MediatR;
using TaskManager.Application.Abstractions.Authentication;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IValidator<CreateUserRequest> _userValidator;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService,
        IValidator<CreateUserRequest> validator, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _userValidator = validator;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserCommand userToCreate, CancellationToken cancellationToken)
    {
        var normalizedEmail = userToCreate.UserToCreate.Email.Trim().ToLowerInvariant();

       var createUser = new CreateUserRequest() { UserName = userToCreate.UserToCreate.UserName, Email = normalizedEmail };

        var resultValidator = await _userValidator.ValidateAsync(createUser, cancellationToken);
        if (!resultValidator.IsValid)
        {
            var errors = resultValidator.Errors.Select(x => x.ErrorMessage).ToList();
            return Result<UserResponse>.Failed(errors, StatusType.ValidationError);
        }

        var user = User.Register(createUser.Email, createUser.UserName);

        var hash = _passwordHasherService.HashPassword(user, userToCreate.UserToCreate.Password);

        user.SetPasswordHash(hash);

       var userCreated = await _userRepository.CreateUserAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        var mappedUsers = _mapper.Map<UserResponse>(userCreated);
        return Result<UserResponse>.Success(mappedUsers);
    }
}
