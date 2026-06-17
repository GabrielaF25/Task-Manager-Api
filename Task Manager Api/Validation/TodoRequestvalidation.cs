using FluentValidation;
using Task_Manager_Api.Dtos;

namespace Task_Manager_Api.Validation;

public class TodoRequestValidation : AbstractValidator<CreateTodoRequest>
{
    public TodoRequestValidation()
    {
        RuleFor(x => x.Title)
          .NotEmpty().WithMessage("The title cannot be empty")
          .MaximumLength(100).WithMessage("The length of the title cannot be long than 100 characteres.");
    }
}
