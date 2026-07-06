using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Application.Features.Users.UpdateUserRole;

public record class UpdateUserRoleCommand(UpdateUserRequest UpdateUserRequest) : IRequest<Result<UserResponse>>;
