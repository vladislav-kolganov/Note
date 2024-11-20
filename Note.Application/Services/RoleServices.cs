using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Note.Application.Resources;
using Note.Domain.Dto.RoleDto;
using Note.Domain.Entity;
using Note.Domain.Enum;
using Note.Domain.Interfaces.Repositories;
using Note.Domain.Interfaces.Services;
using Note.Domain.Result;

namespace Note.Application.Services
{
    public class RoleServices : IRoleService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;

        public RoleServices(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
            if (role != null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleAlreadyExists,
                    ErrorCode = (int)ErrorCodes.RoleAlreadyExists
                };
            }
            role = new Role()
            {
                Name = dto.Name,
            };

            await _roleRepository.CreateAsync(role);
            await _roleRepository.SaveChangeAsync();
            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role)
            };
        }

        public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }
            _roleRepository.Remove(role);
            await _roleRepository.SaveChangeAsync();
            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role)
            };

        }

        public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }
            var updatedRole = _roleRepository.Update(role);
            await _roleRepository.SaveChangeAsync();
            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(updatedRole)
            };
        }

        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll().
                Include(x => x.Role).
                FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                    ErrorMessage = ErrorMessage.UserNotFound,
                };
            }
            var roles = user.Role.Select(x => x.Name).ToArray();
            if (roles.Any(x => x != dto.RoleName))
            {
                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName);
                if (role == null)
                {
                    return new BaseResult<UserRoleDto>
                    {
                        ErrorCode = (int)ErrorCodes.UserNotFound,
                        ErrorMessage = ErrorMessage.UserNotFound,
                    };
                }
                var userRole = new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };
                await _userRoleRepository.CreateAsync(userRole);
                await _userRepository.SaveChangeAsync();
                return new BaseResult<UserRoleDto>
                {
                    Data = new UserRoleDto()
                    {
                        Login = user.Login,
                        RoleName = role.Name,
                    }
                };
            }

            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.UserAlreadyExistsThisRole,
                ErrorCode = (int)ErrorCodes.UserAlreadyExistsThisRole,
            };

        }
    }
}
