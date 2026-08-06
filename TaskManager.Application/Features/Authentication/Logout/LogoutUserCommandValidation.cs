using FluentValidation;
using TaskManager.Application.Features.Authentication.Logout;

public class LogoutUserCommandValidation : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserCommandValidation()
    {
        RuleFor(x => x.Request.RefreshToken)
            .NotEmpty();
    }
}