using FluentValidation;

namespace TaskManager.Application.Features.Users.UpdateUserRole;

public class UpdateUserRoleCommandValidation : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRoleCommandValidation()
    {
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid role.");
    }
}
