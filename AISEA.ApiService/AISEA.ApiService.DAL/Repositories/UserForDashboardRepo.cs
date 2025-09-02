using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class UserForDashboardRepo : GenericRepository<User>
    {
        public UserForDashboardRepo(AiseaContext context) : base(context) { }

        // DTOs for return values
        public record UserStatusCount(EUserStatus Status, int Count);
        public record UserRoleCount(string RoleName, int Count);

        /// <summary>
        /// 1. User count by status (Pie Chart - Admin)
        /// </summary>
        public async Task<List<UserStatusCount>> GetUserCountByStatusAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .GroupBy(u => u.Status)
                .Select(g => new UserStatusCount(g.Key, g.Count()))
                .ToListAsync();
        }

        /// <summary>
        /// 2. User count by role (Bar/Pie Chart - Admin)
        /// </summary>
        public async Task<List<UserRoleCount>> GetUserCountByRoleAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.Role != null)
                .GroupBy(u => u.Role.Name) // assumes Role.Name is available
                .Select(g => new UserRoleCount(g.Key, g.Count()))
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }
    }
}
