using FluentValidation;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.CreateProject;

public class CreateProjectCommandValidation : AbstractValidator<LogoutUserCommandValidation>
{
    public CreateProjectCommandValidation()
    {
        RuleFor(x => x.Project.Name)
          .NotEmpty().WithMessage("The Name cannot be empty")
          .MaximumLength(100).WithMessage("The length of the Name cannot be long than 100 characteres.");

        RuleFor(x => x.Project.Description)
         .MaximumLength(500).WithMessage("The length of the Description cannot be long than 500 characteres.");
    }
}
