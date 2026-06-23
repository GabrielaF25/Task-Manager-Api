using FluentValidation;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Pagination;

namespace TaskManager.Application.Pagination.Validation;

public class PaginationParamValidation : AbstractValidator<PaginationParam>
{
    public PaginationParamValidation()
    {
        RuleFor(p => p.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("The PageSize cannot be less than 1 or greater than 100");

        RuleFor(p => p.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("The PageNumber cannot be less than 1");
    }
}

