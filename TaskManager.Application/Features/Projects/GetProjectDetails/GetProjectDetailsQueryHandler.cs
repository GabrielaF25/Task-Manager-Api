using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.GetProjectDetails;

namespace TaskManager.Application.Features.Projects.Queries.GetProjectDetails;

public class GetProjectDetailsQueryHandler :
    IRequestHandler<GetProjectDetailsQuery, Result<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetProjectDetailsQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<ProjectDto>> Handle(GetProjectDetailsQuery request, CancellationToken ct)
    {
        var projectDomain = await _projectRepository.GetProjectDetailsByIdAsync(request.Id, ct);

        if (projectDomain == null)
        {
            var error = new List<string> { "Project was not found." };
            return Result<ProjectDto>.Failed(error, StatusType.NotFound);
        }

        var ownerId = _currentUserService.GetCurrentUserId();
        if (projectDomain.OwnerId != ownerId)
        {
            var error = new List<string> { "You are not authorize for viewing the project." };
            return Result<ProjectDto>.Failed(error, StatusType.Forbidden);
        }

        return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(projectDomain));
    }
}