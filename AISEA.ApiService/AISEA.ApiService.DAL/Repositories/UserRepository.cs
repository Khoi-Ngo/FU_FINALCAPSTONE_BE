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

        public async Task<User> GetUserWProfileAsync(string username)
        {
            return await _context.Users.Include(u => u.StaffProfile).Include(u => u.StudentProfile)
            .FirstOrDefaultAsync(u => u.IsDeleted == false && u.Username == username && u.Status == EUserStatus.ACTIVE);
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


        public async Task<(List<User> users, int totalCount)> GetStudentsPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users
                .Where(u => u.RoleId == (int)EUserRole.STUDENT)
                .Include(u => u.Role)
                .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(List<User> users, int totalCount)> GetStaffsPagedAsync(int pageNumber, int pageSize, EUserRole staffRole)
        {
            var query = _context.Users
                .Where(u => u.RoleId == (int)staffRole)
                .Include(u => u.Role)
                .Include(u => u.StaffProfile);
            var totalCount = await query.CountAsync();
            var users = await query
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

        public async Task<(List<User> users, int totalCount)> GetActiveAdvisorsPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users
                .Where(u => u.RoleId == (int)EUserRole.ADVISOR && u.IsDeleted == false & u.Status == EUserStatus.ACTIVE)
                .Include(u => u.Role)
                .Include(u => u.StaffProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(object users, int totalCount)> GetActiveStudentsPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users
              .Where(u => u.RoleId == (int)EUserRole.STUDENT && u.IsDeleted == false & u.Status == EUserStatus.ACTIVE)
              .Include(u => u.Role)
              .Include(u => u.StudentProfile);
            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }
    }
}