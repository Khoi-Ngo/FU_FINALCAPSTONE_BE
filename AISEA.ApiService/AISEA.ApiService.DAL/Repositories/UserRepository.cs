using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(AiseaContext context) : base(context)
        {
        }
        //get user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.StudentProfile).Include(u => u.StaffProfile).FirstOrDefaultAsync(u => u.Email == email && u.IsDeleted == false && u.Status == EUserStatus.ACTIVE);
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.StudentProfile).Include(u => u.StaffProfile).FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted == false && u.Status == EUserStatus.ACTIVE);
        }
        public async Task<User> GetUserByEmailOrUsernameAsync(string email, string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.Username == username);
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Status == EUserStatus.ACTIVE && u.IsDeleted == false)
                .Include(u => u.Role)
                .ToListAsync();
        }
        public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users.Include(u => u.Role);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetActiveUsersPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users
                .Where(u => u.Status == EUserStatus.ACTIVE && u.IsDeleted == false)
                .Include(u => u.Role);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<User> GetUserWStudentProfileAsync(string username)
        {
            return await _context.Users.Include(u => u.StudentProfile)
            .FirstOrDefaultAsync(u => u.IsDeleted == false && u.Username == username && u.Status == EUserStatus.ACTIVE && u.RoleId == (int)EUserRole.STUDENT);
        }
        public async Task<List<User>> GetUsersWStudentProfilesAsync(List<string> usernames)
        {
            return await _context.Users.Include(u => u.StudentProfile)
            .Where(u => u.IsDeleted == false && usernames.Contains(u.Username) && u.Status == EUserStatus.ACTIVE && u.RoleId == (int)EUserRole.STUDENT)
            .ToListAsync();
        }


        public async Task<long> GetStudentProfileIdByUsernameAsync(string username)
        {
            var studentProfileId = await _context.Users
                .Where(u =>
                    u.Username == username &&
                    !u.IsDeleted &&
                    u.Status == EUserStatus.ACTIVE &&
                    u.RoleId == (int)EUserRole.STUDENT)
                .Select(u => u.StudentProfile.Id)
                .FirstOrDefaultAsync();

            return studentProfileId;
        }

        public async Task<long> GetStaffProfileIdByUsernameAsync(string username)
        {
            var staffProfileId = await _context.Users
                .Where(u =>
                    u.Username == username &&
                    !u.IsDeleted &&
                    u.Status == EUserStatus.ACTIVE &&
                    u.RoleId != (int)EUserRole.STUDENT)
                .Select(u => u.StaffProfile.Id)
                .FirstOrDefaultAsync();

            return staffProfileId;
        }


        public async Task<(List<User> users, int totalCount)> GetStudentsPagedAsync(int pageNumber, int pageSize, string? search = "")
        {
            var query = _context.Users
                .Where(u => u.RoleId == (int)EUserRole.STUDENT);

            query = query.Include(u => u.Role)
                         .Include(u => u.StudentProfile);

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Email.Contains(search) ||
                    (u.FirstName + " " + u.LastName).Contains(search) ||
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(List<User> users, int totalCount)> GetStaffsPagedAsync(int pageNumber, int pageSize, EUserRole staffRole, string? search = "")
        {
            var query = _context.Users
                .Where(u => u.RoleId == (int)staffRole);

            query = query.Include(u => u.Role)
                         .Include(u => u.StaffProfile);

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Email.Contains(search) ||
                    (u.FirstName + " " + u.LastName).Contains(search) ||
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<User> GetStudentByIdAsync(long id)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.StudentProfile).FirstOrDefaultAsync(u => u.Id == id && u.RoleId == (int)EUserRole.STUDENT);
        }
        public async Task<User> GetStaffByIdAsync(long id)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.StaffProfile).FirstOrDefaultAsync(u => u.Id == id && u.RoleId != (int)EUserRole.STUDENT);

        }

        public async Task<(List<User> users, int totalCount)> GetActiveAdvisorsPagedAsync(int pageNumber, int pageSize, string? search = "")
        {
            var query = _context.Users
                .Where(u => u.RoleId == (int)EUserRole.ADVISOR && u.IsDeleted == false & u.Status == EUserStatus.ACTIVE
                    && u.Username != "AISEABot"
                );

            query = query.Include(u => u.Role)
                         .Include(u => u.StaffProfile);

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Email.Contains(search) ||
                    (u.FirstName + " " + u.LastName).Contains(search) ||
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetActiveStudentsPagedAsync(int pageNumber, int pageSize, string? search = "")
        {
            var query = _context.Users
              .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.IsDeleted == false & u.Status == EUserStatus.ACTIVE);

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Email.Contains(search) ||
                    (u.FirstName + " " + u.LastName).Contains(search) ||
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search));
            }

            query = query.Include(u => u.Role)
                         .Include(u => u.StudentProfile);

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetStudentsByProgramIdPagedAsync(int pageNumber, int pageSize, long programId)
        {
            var query = _context.Users
                .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.StudentProfile.ProgramId == programId)
                .Include(u => u.Role)
                .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetAllStudentsByComboCodePagedAsync(int pageNumber, int pageSize, string comboCode)
        {
            var query = _context.Users
               .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.StudentProfile.RegisteredComboCode == comboCode)
               .Include(u => u.Role)
               .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetStudentsByCurriculumCodePagedAsync(int pageNumber, int pageSize, string curriculumCode)
        {
            var query = _context.Users
            .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.StudentProfile.CurriculumCode == curriculumCode)
            .Include(u => u.Role)
            .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetAllActiveStudentsByComboCodePagedAsync(int pageNumber, int pageSize, string comboCode)
        {
            var query = _context.Users
             .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.StudentProfile.RegisteredComboCode == comboCode && u.Status == EUserStatus.ACTIVE && u.IsDeleted == false)
             .Include(u => u.Role)
             .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetAllActiveStudentsByProgramIdPagedAsync(int pageNumber, int pageSize, long programId)
        {
            var query = _context.Users
             .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.StudentProfile.ProgramId == programId && u.Status == EUserStatus.ACTIVE && u.IsDeleted == false)
             .Include(u => u.Role)
             .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetAllActiveStudentsByCurriculumCodePagedAsync(int pageNumber, int pageSize, string curriculumCode)
        {
            var query = _context.Users
             .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.StudentProfile.CurriculumCode == curriculumCode && u.Status == EUserStatus.ACTIVE && u.IsDeleted == false)
             .Include(u => u.Role)
             .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<List<long>> GetAllActiveStudentUserIDsAsync()
        {
            var studentIds = await _context.Users
                .Where(u => u.RoleId == (int)EUserRole.STUDENT
                            && u.Status == EUserStatus.ACTIVE
                            && !u.IsDeleted)
                .Select(u => u.Id)
                .ToListAsync();

            return studentIds;
        }

 
    }
}