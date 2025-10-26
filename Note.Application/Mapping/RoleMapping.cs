using AutoMapper;
using Note.Domain.Dto.RoleDto;
using Note.Domain.Entity;

namespace Note.Application.Mapping
{
    /// <summary>
    /// Маппер для ролей.
    /// </summary>
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
