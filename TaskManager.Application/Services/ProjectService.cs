using AutoMapper;
using FluentValidation;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.Queries;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProjectRequest> _validatorCreate;
    private readonly IValidator<PaginationParam> _paginationValidator;


    public ProjectService(IProjectRepository projectRepository, IMapper mapper,
        IValidator<CreateProjectRequest> validator, IValidator<PaginationParam> paginationValidator)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _validatorCreate = validator;
        _paginationValidator = paginationValidator;
    }

    public async Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectRequest project, CancellationToken ct)
    {
        var validationResult = await _validatorCreate.ValidateAsync(project, ct);

        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            return Result<ProjectDto>.Failed(error, StatusType.ValidationError);
        }

        var projectDomain = _mapper.Map<Project>(project);
        var createdProject = await _projectRepository.AddAsync(projectDomain, ct);

        await _projectRepository.SaveChangesAsync(ct);

        return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(createdProject));

    }

    public async Task<Result<ProjectDto>> GetProjectDetailsByIdAsync(int id, CancellationToken ct)
    {
        var projectDomain = await _projectRepository.GetProjectDetailsByIdAsync(id, ct);
        if(projectDomain == null)
        {
            var error = new List<string> { "Project was not found." };
            return Result<ProjectDto>.Failed(error, StatusType.NotFound);
        }

        return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(projectDomain));
    }

    public async Task<Result<PaginationResult<ProjectDto>>> GetProjectsAsync(QueryParamProject queryParam, PaginationParam pagination,CancellationToken ct)
    {
        var result = await _paginationValidator.ValidateAsync(pagination, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<PaginationResult<ProjectDto>>.Failed(errors, StatusType.ValidationError);
        }

        var projectPaginated = await _projectRepository.GetProjectsAsync(queryParam, pagination,ct);

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

    public async Task<Result>RemoveAsync(int id, CancellationToken ct)
    {
        var project = await _projectRepository.GetProjectByIdAsync(id, ct);

        if (project == null) 
        {
            var error = new List<string> { "The project was not found" };
            return Result.Failed(error, StatusType.NotFound);
        }

        _projectRepository.Remove(project);
        await _projectRepository.SaveChangesAsync(ct);

        return Result.Success();

    }
}
