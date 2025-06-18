using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Role;
using AISEA.ApiService.SHARED.DTOs.Responses.Role;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Role
{
    public class RoleService
    {
        private readonly RoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RoleService(RoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task CreateRoleAsync(CreateRoleRequest request)
        {
            var role = _mapper.Map<DAL.Entities.Role>(request);
            await _roleRepository.CreateAsync(role);
        }

        public async Task UpdateRoleAsync(long roleId, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }

            role = _mapper.Map(request, role);
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeleteRoleAsync(long roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }

            await _roleRepository.RemoveAsync(role);
        }

        public async Task<List<GetRoleResponse>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<List<GetRoleResponse>>(roles);
        }

        public async Task<GetRoleResponse> GetRoleByIdAsync(long roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            return _mapper.Map<GetRoleResponse>(role);
        }
    }
}