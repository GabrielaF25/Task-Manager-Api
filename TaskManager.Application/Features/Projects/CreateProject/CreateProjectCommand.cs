using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.CreateProject;

public record LogoutUserCommandValidation(CreateProjectRequest Project) : IRequest<Result<ProjectDto>>;
