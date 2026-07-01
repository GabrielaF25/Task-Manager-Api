using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Authentication.Dtos;

namespace TaskManager.Application.Features.Authentication.Logout;

public record LogoutUserCommand(RefreshTokenRequest Request) : IRequest<Result>;
