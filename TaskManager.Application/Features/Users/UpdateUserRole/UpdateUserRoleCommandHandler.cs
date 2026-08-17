using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Users.UpdateUserRole;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateUserRoleCommandHandler(IUserRepository userRepository,ICurrentUserService currentUserService, IMapper mapper)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }
    public async Task<Result<UserResponse>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UpdateUserRequest.Id, cancellationToken);

        if (user == null)
        {
            return Result<UserResponse>.Failed(["User not found"], StatusType.NotFound);
        }

        var currentUser = await _userRepository.GetUserByIdAsync(_currentUserService.GetCurrentUserId(), cancellationToken);

        if (currentUser == null || currentUser.UserRole != UserRole.Admin)
        {
            return Result<UserResponse>.Failed(["Unauthorized"], StatusType.Unauthorized);
        }

        user.ChangeRole(request.UpdateUserRequest.Role);   

        var useResponse = _mapper.Map<UserResponse>(user);

        return Result<UserResponse>.Success(useResponse);
    }
}
