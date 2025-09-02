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
        public record UserRegistrationTrend(DateTime Month, int Count);
        public record StudentProgramCount(string ProgramName, int StudentCount);
        public record StaffDepartmentCount(string Department, int StaffCount);

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
        /// 2. User registration trend (Line Chart - Admin)
        /// </summary>
        public async Task<List<UserRegistrationTrend>> GetUserRegistrationTrendAsync(int monthsBack = 12)
        {
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack);

            return await _context.Users
                .Where(u => !u.IsDeleted && u.CreatedAt >= startDate)
                .GroupBy(u => new { u.CreatedAt!.Value.Year, u.CreatedAt.Value.Month })
                .Select(g => new UserRegistrationTrend(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Count()
                ))
                .OrderBy(x => x.Month)
                .ToListAsync();
        }

        /// <summary>
        /// 3. Student enrollment by program (Bar Chart - Admin)
        /// </summary>
        public async Task<List<StudentProgramCount>> GetStudentCountByProgramAsync()
        {
            return await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted && sp.Program != null)
                .GroupBy(sp => sp.Program!.ProgramName)
                .Select(g => new StudentProgramCount(g.Key, g.Count()))
                .OrderByDescending(x => x.StudentCount)
                .ToListAsync();
        }

        /// <summary>
        /// 4. Staff distribution by department (Pie Chart - Admin)
        /// </summary>
        public async Task<List<StaffDepartmentCount>> GetStaffCountByDepartmentAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted && sp.Department != null)
                .GroupBy(sp => sp.Department!)
                .Select(g => new StaffDepartmentCount(g.Key, g.Count()))
                .OrderByDescending(x => x.StaffCount)
                .ToListAsync();
        }
    }
}
