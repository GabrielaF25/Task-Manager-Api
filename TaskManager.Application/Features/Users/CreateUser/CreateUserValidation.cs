using FluentValidation;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Application.Features.Users.CreateUser;

public class CreateUserValidation : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidation(IUserLookupService userLookupService)
    {
        RuleFor(u => u.UserToCreate.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(50)
            .MustAsync(async (userName, ct)
            => !await userLookupService.UserNameExistsAsync(userName.Trim().ToLowerInvariant(), ct))
             .WithMessage("UserName is already registered");

        RuleFor(u => u.UserToCreate.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100)
            .WithMessage("Password cannot be greater than 100 characters.")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(u => u.UserToCreate.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("The format of email is wrong")
            .MaximumLength(255)
            .MustAsync(async (email, ct) =>
                !await userLookupService.EmailExistsAsync(email.Trim().ToLowerInvariant(), ct))
            .WithMessage("Email is already registered");

    }
}
