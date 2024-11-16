using AutoMapper;
using Note.Domain.Dto.RoleDto;
using Note.Domain.Dto.UserDto;
using Note.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Application.Mapping
{
    public class RoleMapping: Profile
    {
        public RoleMapping()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
