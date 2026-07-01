using FluentValidation;
using TaskManager.Application.Features.Authentication.Logout;

public class LogoutUserComandValidation : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserComandValidation()
    {
        RuleFor(x => x.Request.RefreshToken)
            .NotEmpty();
    }
}