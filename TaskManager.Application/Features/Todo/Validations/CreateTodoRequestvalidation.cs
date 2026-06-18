using FluentValidation;
using TaskManager.Application.Features.Todo.Dtos;

namespace TaskManager.Application.Features.Todo.Validations;

public class CreateTodoRequestvalidation : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestvalidation()
    {
        RuleFor(x => x.Title)
          .NotEmpty().WithMessage("The title cannot be empty")
          .MaximumLength(100).WithMessage("The length of the title cannot be long than 100 characteres.");
    }
}
