using FluentValidation;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.CreateTodo;

public class CreateTodoRequestvalidation : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoRequestvalidation()
    {
        RuleFor(x => x.TodoRequest.Title)
          .NotEmpty().WithMessage("The title cannot be empty")
          .MaximumLength(100).WithMessage("The length of the title cannot be long than 100 characteres.");
    }
}
