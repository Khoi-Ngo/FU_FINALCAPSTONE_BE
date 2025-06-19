using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.DTOs.Responses.User;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;
using BC = BCrypt.Net.BCrypt;

namespace AISEA.ApiService.BAL.Services.User
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(UserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task CreateUserAsync(CreateUserRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailOrUsernameAsync(request.Email, request.Username);
            if (existingUser is not null)
            {
                throw new InvalidUserCreatedException("User with this email or username already exists.");
            }

            var user = _mapper.Map<DAL.Entities.User>(request);
            user.Password = BC.EnhancedHashPassword(request.Password);
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.CreateAsync(user);
        }

        public async Task CreateUsersAsync(List<CreateUserRequest> requests)
        {
            foreach (var request in requests)
            {
                await CreateUserAsync(request);
            }
        }
        public async Task<List<GetUserListResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<List<GetUserListResponse>>(users);
        }

        public async Task<List<GetUserListResponse>> GetAllActiveUsersAsync()
        {
            var users = await _userRepository.GetActiveUsersAsync();
            return _mapper.Map<List<GetUserListResponse>>(users);
        }
        public async Task<GetUserDetailResponse> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }

            return _mapper.Map<GetUserDetailResponse>(user);
        }

        public async Task UpdateUserAsync(long id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }

            _mapper.Map(request, user);
            await _userRepository.UpdateAsync(user);
        }

        public async Task DisableUserAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }
            if (user.IsDeleted || user.Status == EUserStatus.INACTIVE)
            {
                throw new InvalidOperationException("User is already disabled.");
            }
            user.Status = EUserStatus.INACTIVE;
            user.DeletedAt = DateTime.UtcNow;
            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);
        }
    }
}