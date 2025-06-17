using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
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
            if (existingUser != null)
            {
                throw new InvalidUserCreatedException("User with this email or username already exists.");
            }

            var user = _mapper.Map<DAL.Entities.User>(request);
            user.Password = BC.EnhancedHashPassword(request.Password);
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.CreateAsync(user);
        }
    }
}