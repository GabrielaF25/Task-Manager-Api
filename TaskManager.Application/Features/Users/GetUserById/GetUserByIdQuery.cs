using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Application.Features.Users.GetUser;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserResponse>>;
