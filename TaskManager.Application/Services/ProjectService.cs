using AutoMapper;
using FluentValidation;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.Queries;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProjectRequest> _validatorCreate;
    private readonly IValidator<PaginationParam> _validatorPagination;


    public ProjectService(IProjectRepository projectRepository, IMapper mapper,
        IValidator<CreateProjectRequest> validator, IValidator<PaginationParam> validatorPagination)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _validatorCreate = validator;
        _validatorPagination = validatorPagination;
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectRequest project, CancellationToken ct)
    {
        await _validatorCreate.ValidateAndThrowAsync(project, ct);
        var projectDomain = _mapper.Map<Project>(project);
        var createdProject = await _projectRepository.AddAsync(projectDomain, ct);

        await _projectRepository.SaveChangesAsync(ct);

        return _mapper.Map<ProjectDto>(createdProject);

    }

    public async Task<ProjectDto?> GetProjectDetailsByIdAsync(int id, CancellationToken ct)
    {
        var projectDomain = await _projectRepository.GetProjectDetailsByIdAsync(id, ct);

        return _mapper.Map<ProjectDto>(projectDomain);
    }

    public async Task<PaginationResult<ProjectDto>> GetProjectsAsync(QueryParamProject queryParam, PaginationParam pagination,CancellationToken ct)
    {
        await _validatorPagination.ValidateAndThrowAsync(pagination, ct);

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

       
        return paginatedProjectDto;

    }

    public async Task<bool>  RemoveAsync(int id, CancellationToken ct)
    {
        var project = await _projectRepository.GetProjectByIdAsync(id, ct);

        if (project == null) return false;

       _projectRepository.Remove(project);
        await _projectRepository.SaveChangesAsync(ct);

        return true;

    }

}
