using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Application.Features.Users.CreateUser;

public record CreateUserCommand(CreateUserRequest  UserToCreate) : IRequest<Result>;

