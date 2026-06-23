using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.Queries.GetProjectDetails;

public class GetProjectDetailsQueryHandler :
    IRequestHandler<GetProjectDetailsQuery, Result<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetProjectDetailsQueryHandler(IProjectRepository projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
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

        return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(projectDomain));
    }
}