using AutoMapper;
using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;

namespace Note.Application.Mapping;

/// <summary>
/// Маппер для Пользователей.
/// </summary>
public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}

