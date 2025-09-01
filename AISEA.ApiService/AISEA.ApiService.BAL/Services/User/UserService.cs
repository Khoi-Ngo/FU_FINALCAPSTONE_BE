using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.User;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;
using BC = BCrypt.Net.BCrypt;

namespace AISEA.ApiService.BAL.Services.User
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly StudentProfileRepository _studentProfileRepository;
        private readonly StaffProfileRepository _staffProfileRepository;
        private readonly IMapper _mapper;
        private readonly IJWTService _jWTService;

        public UserService(UserRepository userRepository, IMapper mapper, StudentProfileRepository studentProfileRepository, StaffProfileRepository staffProfileRepository, IJWTService jWTService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _staffProfileRepository = staffProfileRepository;
            _studentProfileRepository = studentProfileRepository;
            _jWTService = jWTService;
        }

        #region Create user
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

        public async Task CreateUsersAsync(List<BulkCreateStudentRequest> requests)
        {
            var newUsers = _mapper.Map<List<CreateUserRequest>>(requests);
            newUsers.ForEach(u => u.RoleId = (long)EUserRole.STUDENT);
            await CreateUsersAsync(newUsers);
        }

        public async Task CreateUsersAsync(List<BulkCreateStaffByRoleRequest> requests, EUserRole staffRole)
        {
            var newUsers = _mapper.Map<List<CreateUserRequest>>(requests);
            newUsers.ForEach(u => u.RoleId = (long)staffRole);
            await CreateUsersAsync(newUsers);
        }

        #endregion


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
        public async Task<GetStudentDetailResponse> GetStudentByIdAsync(long id)
        {
            var student = await _userRepository.GetStudentByIdAsync(id);
            if (student is null)
            {
                throw new NotFoundException("Student not found.");
            }

            return _mapper.Map<GetStudentDetailResponse>(student);
        }

        public async Task UpdateUserAsync(long id, UpdateStudentRequest request, string accessToken)
        {
            var student = await _userRepository.GetStudentByIdAsync(id);
            if (student is null)
            {
                throw new NotFoundException("Student not found.");
            }
            if (!IsValidAccessUpdate(accessToken, student)) throw new InvalidAccessUserException("No permission to edit this user");

            _mapper.Map(request, student);
            if (request.StudentDataUpdateRequest is not null)
            {
                var updateProfile = _mapper.Map(request.StudentDataUpdateRequest, student.StudentProfile);
                await _studentProfileRepository.UpdateAsync(updateProfile);

            }
            await _userRepository.UpdateAsync(student);
        }

        private bool IsValidAccessUpdate(string accessToken, DAL.Entities.User updatedUser)
        {
            if (_jWTService.GetRoleIdFromToken(accessToken) == (long)EUserRole.ADMIN) return true;
            return updatedUser.Id == _jWTService.GetUserIdFromToken(accessToken);
        }

        public async Task UpdateUserAsync(long id, UpdateStaffRequest request, string accessToken)
        {
            var staff = await _userRepository.GetStaffByIdAsync(id);
            if (staff is null)
            {
                throw new NotFoundException("Staff not found.");
            }
            if (!IsValidAccessUpdate(accessToken, staff)) throw new InvalidAccessUserException("No permission to edit this user");

            _mapper.Map(request, staff);
            if (request.StaffDataUpdateRequest is not null)
            {
                var updateProfile = _mapper.Map(request.StaffDataUpdateRequest, staff.StaffProfile);
                await _staffProfileRepository.UpdateAsync(updateProfile);
            }
            await _userRepository.UpdateAsync(staff);
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
        public async Task<PagedResult<GetUserListResponse>> GetAllUsersPagedAsync(PaginationRequest request)
        {
            var (users, totalCount) = await _userRepository.GetUsersPagedAsync(request.PageNumber, request.PageSize);
            return new PagedResult<GetUserListResponse>
            {
                Items = _mapper.Map<List<GetUserListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        public async Task<PagedResult<GetUserListResponse>> GetAllActiveUsersPagedAsync(PaginationRequest request)
        {
            var (users, totalCount) = await _userRepository.GetActiveUsersPagedAsync(request.PageNumber, request.PageSize);
            return new PagedResult<GetUserListResponse>
            {
                Items = _mapper.Map<List<GetUserListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<GetStudentListResponse>> GetAllStudentsPagedAsync(GetUsersWithSearchRequest request)
        {
            var (users, totalCount) = await _userRepository.GetStudentsPagedAsync(request.PageNumber, request.PageSize, request.Search);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<GetStaffListResponse>> GetAllStaffsPagedAsync(GetUsersWithSearchRequest request, EUserRole staffRole)
        {
            var (users, totalCount) = await _userRepository.GetStaffsPagedAsync(request.PageNumber, request.PageSize, staffRole, request.Search);
            return new PagedResult<GetStaffListResponse>
            {
                Items = _mapper.Map<List<GetStaffListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        public async Task<PagedResult<GetStaffListResponse>> GetAllActiveAdvisorsAsync(GetUsersWithSearchRequest request)
        {
            var (users, totalCount) = await _userRepository.GetActiveAdvisorsPagedAsync(request.PageNumber, request.PageSize, request.Search);
            return new PagedResult<GetStaffListResponse>
            {
                Items = _mapper.Map<List<GetStaffListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetStaffDetailResponse> GetStaffByIdAsync(long id)
        {
            var staff = await _userRepository.GetStaffByIdAsync(id);
            if (staff is null)
            {
                throw new NotFoundException("Staff not found.");
            }

            return _mapper.Map<GetStaffDetailResponse>(staff);
        }

        public async Task ResetNumberOfBanAsync(long studentProfileId)
        {
            var studentProfile = await _studentProfileRepository.GetByIdAsync(studentProfileId);
            studentProfile.NumberOfBan = 0;
            await _studentProfileRepository.UpdateAsync(studentProfile);
        }

        public async Task UpdateAvatarAsync(string accessToken, UpdateAvatarRequest request)
        {
            var userId = _jWTService.GetUserIdFromToken(accessToken);
            var user = await _userRepository.GetByIdAsync(userId);
            user.AvatarUrl = request.URL;
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateAvatarAsync(long userId, UpdateAvatarRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            user.AvatarUrl = request.URL;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<PagedResult<GetStudentListResponse>> GetAllActiveStudentsAsync(GetActiveStudentsRequest request)
        {
            var (users, totalCount) = await _userRepository.GetActiveStudentsPagedAsync(request.PageNumber, request.PageSize, request.Search);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
   
        public async Task<PagedResult<GetStudentListResponse>> GetAllStudentsByComboCodePagedAsync(PaginationRequest request, string comboCode)
        {
            var (users, totalCount) = await _userRepository.GetAllStudentsByComboCodePagedAsync(request.PageNumber, request.PageSize, comboCode);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<GetStudentListResponse>> GetAllStudentsByProgramIdPagedAsync(PaginationRequest request, long programId)
        {
            var (users, totalCount) = await _userRepository.GetStudentsByProgramIdPagedAsync(request.PageNumber, request.PageSize, programId);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<GetStudentListResponse>> GetAllStudentsByCurriculumCodePagedAsync(PaginationRequest request, string curriculumCode)
        {
            var (users, totalCount) = await _userRepository.GetStudentsByCurriculumCodePagedAsync(request.PageNumber, request.PageSize, curriculumCode);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<GetStudentListResponse>> GetAllActiveStudentsByComboCodePagedAsync(PaginationRequest request, string comboCode)
        {
            var (users, totalCount) = await _userRepository.GetAllActiveStudentsByComboCodePagedAsync(request.PageNumber, request.PageSize, comboCode);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<GetStudentListResponse>> GetAllActiveStudentsByProgramIdPagedAsync(PaginationRequest request, long programId)
        {
            var (users, totalCount) = await _userRepository.GetAllActiveStudentsByProgramIdPagedAsync(request.PageNumber, request.PageSize, programId);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<GetStudentListResponse>> GetAllActiveStudentsByCurriculumCodePagedAsync(PaginationRequest request, string curriculumCode)
        {
            var (users, totalCount) = await _userRepository.GetAllActiveStudentsByCurriculumCodePagedAsync(request.PageNumber, request.PageSize, curriculumCode);
            return new PagedResult<GetStudentListResponse>
            {
                Items = _mapper.Map<List<GetStudentListResponse>>(users),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}