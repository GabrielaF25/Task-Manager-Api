using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Application.Features.Users.GetUser;
using TaskManager.Application.Features.Users.LoginUser;

namespace Task_Manager_Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateUserCommand(request), cancellationToken);

        return HandleCreatedResult("GetUserById", result, dto => new { id = dto.Id });
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<ActionResult<UserResponse>> CreateUser(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("login")]

    public async Task<ActionResult<LoginResponse>> LoginUser([FromBody]UserCredentials userCredentials, CancellationToken  cancellationToken)
    {
        var result = await _mediator.Send(new LoginUserCommand(userCredentials), cancellationToken);

        return HandleResult(result);
    }
}
