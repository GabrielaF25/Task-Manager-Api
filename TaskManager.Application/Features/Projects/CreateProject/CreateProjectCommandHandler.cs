using AutoMapper;
using FluentValidation;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Projects.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand,Result<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;


    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand createProjectCommand, CancellationToken ct)
    {
        var ownerId = _currentUserService.GetCurrentUserId();

        var projectDomain =
            Project.Create(createProjectCommand.Project.Name, createProjectCommand.Project.Description, ownerId);

        var createdProject = await _projectRepository.AddAsync(projectDomain, ct);

        return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(createdProject));

    }
}
