using FluentValidation;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Features.Users.Dtos;

namespace TaskManager.Application.Features.Users.CreateUser;

public class CreateUserValidation : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidation(IUserLookupService userLookupService)
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(50);

        RuleFor(u => u.Password)
            .MinimumLength(8)
            .MaximumLength(100)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(u => u.Email)
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
