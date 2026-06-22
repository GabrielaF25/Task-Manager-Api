using FluentValidation;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.Validation;

public class CreateProjectValidation : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectValidation()
    {
        RuleFor(x => x.Name)
          .NotEmpty().WithMessage("The Name cannot be empty")
          .MaximumLength(100).WithMessage("The length of the Name cannot be long than 100 characteres.");

        RuleFor(x => x.Description)
         .MaximumLength(500).WithMessage("The length of the Description cannot be long than 500 characteres.");
    }
}
