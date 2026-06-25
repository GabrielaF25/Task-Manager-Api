using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Application.Features.Users.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserResponse>>
{

    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            var error = new List<string> { " The user was not found." };
            return Result<UserResponse>.Failed(error, StatusType.NotFound);
        }

        var userResponse = _mapper.Map<UserResponse>(user);

        return Result<UserResponse>.Success(userResponse);
    }
}
