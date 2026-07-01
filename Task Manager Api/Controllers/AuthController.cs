using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;
using TaskManager.Application.Features.Authentication.Login.LoginUser;
using TaskManager.Application.Features.Authentication.LoginUser;
using TaskManager.Application.Features.Authentication.RefreshTokens.CreateRefreshToken;
using TaskManager.Application.Features.RefreshTokens.CreateRefreshToken;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Application.Features.Users.GetUser;
using TaskManager.Domain.Entities;

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


    [HttpPost("refresh")]

    public async Task<ActionResult<RefreshTokenResponse>> GetRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request), cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("logout")]

    public async Task<ActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest token, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RevokeRefreshToken(token), cancellationToken);

        return HandleResult(result);
    }
}
