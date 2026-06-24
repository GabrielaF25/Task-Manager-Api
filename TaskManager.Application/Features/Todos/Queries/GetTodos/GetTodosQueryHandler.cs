using AutoMapper;
using FluentValidation;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;

namespace TaskManager.Application.Features.Todos.Queries.GetTodos;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, Result<PaginationResult<TodoResponse>>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<PaginationParam> _paginationValidator;

    public GetTodosQueryHandler(ITodoRepository todoRepository, IMapper mapper, IValidator<PaginationParam> param)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
        _paginationValidator = param;
    }

    public async Task<Result<PaginationResult<TodoResponse>>> Handle(GetTodosQuery getTodos , CancellationToken ct)
    {
        var validationResult = await _paginationValidator.ValidateAsync(getTodos.Pagination, ct);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            return Result<PaginationResult<TodoResponse>>.Failed(
                errors,
                StatusType.ValidationError);
        }
        var itemsPageResult = await _todoRepository.GetAllAsync(getTodos.QueryParam, getTodos.Pagination, ct);

        var pageResult = new PaginationResult<TodoResponse>()
        {
            PageNumber = itemsPageResult.PageNumber,
            PageSize = itemsPageResult.PageSize,
            TotalCount = itemsPageResult.TotalCount,
            TotalPages = itemsPageResult.TotalPages,
            HasNextPage = itemsPageResult.HasNextPage,
            HasPreviousPage = itemsPageResult.HasPreviousPage,
            Items = _mapper.Map<IEnumerable<TodoResponse>>(itemsPageResult.Items)
        };

        return Result<PaginationResult<TodoResponse>>.Success(pageResult);
    }
}
