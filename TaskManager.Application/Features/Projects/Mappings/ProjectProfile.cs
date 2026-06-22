using AutoMapper;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Projects.Mappings;

public class ProjectProfile : Profile
{

    public ProjectProfile()
    {
        CreateMap<Project, ProjectDto>().ReverseMap();
        CreateMap<CreateProjectRequest, Project>();
    }
}
