using AutoMapper;
using TaskManager.Application.Features.Users.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Users.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>();
    }
}
