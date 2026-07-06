using FluentValidation;
using TaskManager.Application.Features.Authentication.RefreshTokens;

namespace TaskManager.Application.Features.Authentication.RefreshTokens;

public class RefreshTokenCommandValidation : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidation()
    {
        RuleFor(x => x.Request.RefreshToken)
            .NotEmpty();
    }
}
