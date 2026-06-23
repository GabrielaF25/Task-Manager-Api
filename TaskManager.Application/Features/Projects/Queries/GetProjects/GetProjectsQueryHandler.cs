using AutoMapper;
using FluentValidation;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.Queries.GetProjects
{
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectQuery, Result<PaginationResult<ProjectDto>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<PaginationParam> _paginationValidator;


        public GetProjectsQueryHandler(IProjectRepository projectRepository, IMapper mapper
            , IValidator<PaginationParam> paginationValidator)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _paginationValidator = paginationValidator;
        }

        public async Task<Result<PaginationResult<ProjectDto>>> Handle(GetProjectQuery getProject  , CancellationToken ct)
        {
            var result = await _paginationValidator.ValidateAsync(getProject.Pagination, ct);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<PaginationResult<ProjectDto>>.Failed(errors, StatusType.ValidationError);
            }

            var projectPaginated = await _projectRepository.GetProjectsAsync(getProject.QueryParam, getProject.Pagination, ct);

            var paginatedProjectDto = new PaginationResult<ProjectDto>()
            {
                PageNumber = projectPaginated.PageNumber,
                PageSize = projectPaginated.PageSize,
                TotalCount = projectPaginated.TotalCount,
                TotalPages = projectPaginated.TotalPages,
                HasNextPage = projectPaginated.HasNextPage,
                HasPreviousPage = projectPaginated.HasPreviousPage,
                Items = _mapper.Map<IEnumerable<ProjectDto>>(projectPaginated.Items)
            };

            return Result<PaginationResult<ProjectDto>>.Success(paginatedProjectDto);
        }   
    }

}
