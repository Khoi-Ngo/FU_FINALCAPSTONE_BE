using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class UserForDashboardRepo : GenericRepository<User>
    {
        public UserForDashboardRepo(AiseaContext context) : base(context)
        {
        }

        // DTOs for return values
        public record UserRegistrationTrend(DateTime Month, int Count);
        public record StudentProgramCount(string ProgramName, int StudentCount);
        public record StudentBanInfo(string StudentName, int NumberOfBans);
        public record RoleAgeInfo(string RoleName, double AverageAge);
        public record TopProgramInfo(string ProgramName, int StudentCount, DateTime LatestEnrollment);
        public record StaffTenureInfo(string StaffName, int YearsOfService);
        public record UserRoleDistribution(string RoleName, int UserCount);
        public record StudentEnrollmentByYear(int Year, int StudentCount);
        public record StaffDepartmentWorkload(string Department, int StaffCount, double AvgYearsOfService);
        public record UserActivitySummary(string UserName, string RoleName, int ProfileCompleteness);
        public record StudentCareerGoalDistribution(string CareerGoal, int StudentCount);
        public record StaffCampusDistribution(string Campus, int StaffCount);
        public record UserGenderDistribution(string Gender, int UserCount);
        public record StudentProgramProgress(string ProgramName, int TotalStudents, double AvgBanCount);
        public record StaffPositionSummary(string Position, int StaffCount, DateTimeOffset? EarliestStart);
        public record UserCreationByQuarter(int Year, int Quarter, int UserCount);

        // 1. User count by status for pie chart
        public async Task<Dictionary<EUserStatus, int>> GetUserCountByStatusAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .GroupBy(u => u.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.Status, v => v.Count);
        }

        // 2. User registration trend over time for line chart
        public async Task<List<UserRegistrationTrend>> GetUserRegistrationTrendAsync(int monthsBack = 12)
        {
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack);
            return await _context.Users
                .Where(u => !u.IsDeleted && u.CreatedAt >= startDate)
                .GroupBy(u => new { Year = u.CreatedAt!.Value.Year, Month = u.CreatedAt!.Value.Month })
                .Select(g => new UserRegistrationTrend(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Count()
                ))
                .OrderBy(x => x.Month)
                .ToListAsync();
        }

        // 3. Student enrollment by program for bar chart
        public async Task<List<StudentProgramCount>> GetStudentCountByProgramAsync()
        {
            return await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted && sp.Program != null)
                .GroupBy(sp => sp.Program!.ProgramName) // Assume ProgramName exists
                .Select(g => new StudentProgramCount(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.StudentCount)
                .ToListAsync();
        }

        // 4. Staff distribution by department for pie chart
        public async Task<Dictionary<string, int>> GetStaffCountByDepartmentAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted)
                .GroupBy(sp => sp.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.Department, v => v.Count);
        }

        // 5. Active students with bans for table
        public async Task<List<StudentBanInfo>> GetStudentsWithBansAsync()
        {
            return await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted && sp.NumberOfBan > 0)
                .Select(sp => new StudentBanInfo(
                    sp.User.FirstName + " " + sp.User.LastName,
                    sp.NumberOfBan
                ))
                .OrderByDescending(x => x.NumberOfBans)
                .ToListAsync();
        }

        // 6. Average age of users by role for bar chart
        public async Task<List<RoleAgeInfo>> GetAverageAgeByRoleAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.DateOfBirth != null)
                .GroupBy(u => u.Role.Name)
                .Select(g => new RoleAgeInfo(
                    g.Key,
                    g.Average(u => DateTimeOffset.UtcNow.Year - u.DateOfBirth!.Value.Year)
                ))
                .OrderBy(x => x.RoleName)
                .ToListAsync();
        }

        // 7. Top 5 active programs by student enrollment for table
        public async Task<List<TopProgramInfo>> GetTopActiveProgramsAsync()
        {
            return await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted && sp.Program != null)
                .GroupBy(sp => sp.Program!.ProgramName) // Assume ProgramName exists
                .Select(g => new TopProgramInfo(
                    g.Key,
                    g.Count(),
                    g.Max(sp => sp.EnrolledAt).UtcDateTime
                ))
                .OrderByDescending(x => x.StudentCount)
                .Take(5)
                .ToListAsync();
        }

        // 8. Staff tenure by years of service for bar chart
        public async Task<List<StaffTenureInfo>> GetStaffTenureAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted && sp.StartWorkAt != null)
                .Select(sp => new StaffTenureInfo(
                    sp.User.FirstName + " " + sp.User.LastName,
                    DateTimeOffset.UtcNow.Year - sp.StartWorkAt!.Value.Year
                ))
                .OrderByDescending(x => x.YearsOfService)
                .ToListAsync();
        }

        // 9. User role distribution for pie chart
        public async Task<List<UserRoleDistribution>> GetUserRoleDistributionAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .GroupBy(u => u.Role.Name)
                .Select(g => new UserRoleDistribution(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.UserCount)
                .ToListAsync();
        }

        // 10. Student enrollment by year for line chart
        public async Task<List<StudentEnrollmentByYear>> GetStudentEnrollmentByYearAsync()
        {
            return await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted)
                .GroupBy(sp => sp.EnrolledAt.Year)
                .Select(g => new StudentEnrollmentByYear(
                    g.Key,
                    g.Count()
                ))
                .OrderBy(x => x.Year)
                .ToListAsync();
        }

        // 11. Staff department workload summary for table
        public async Task<List<StaffDepartmentWorkload>> GetStaffDepartmentWorkloadAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted && sp.StartWorkAt != null)
                .GroupBy(sp => sp.Department)
                .Select(g => new StaffDepartmentWorkload(
                    g.Key,
                    g.Count(),
                    g.Average(sp => DateTimeOffset.UtcNow.Year - sp.StartWorkAt!.Value.Year)
                ))
                .OrderByDescending(x => x.StaffCount)
                .ToListAsync();
        }

        // 12. User activity summary for table
        public async Task<List<UserActivitySummary>> GetUserActivitySummaryAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserActivitySummary(
                    u.FirstName + " " + u.LastName,
                    u.Role.Name,
                    (u.AvatarUrl != null ? 20 : 0) + 
                    (u.DateOfBirth != null ? 20 : 0) + 
                    (u.Email != null ? 20 : 0) + 
                    (u.FirstName != null ? 20 : 0) + 
                    (u.LastName != null ? 20 : 0)
                ))
                .OrderByDescending(x => x.ProfileCompleteness)
                .Take(10)
                .ToListAsync();
        }

        // 13. Student career goal distribution for pie chart
        public async Task<List<StudentCareerGoalDistribution>> GetStudentCareerGoalDistributionAsync()
        {
            return await _context.StudentProfiles
                .Where(sp => !sp.IsDeleted && !string.IsNullOrEmpty(sp.CareerGoal))
                .GroupBy(sp => sp.CareerGoal!)
                .Select(g => new StudentCareerGoalDistribution(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.StudentCount)
                .ToListAsync();
        }

        // 14. Staff campus distribution for bar chart
        public async Task<List<StaffCampusDistribution>> GetStaffCampusDistributionAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted)
                .GroupBy(sp => sp.Campus)
                .Select(g => new StaffCampusDistribution(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.StaffCount)
                .ToListAsync();
        }

        // 15. User creation by quarter for line chart
        public async Task<List<UserCreationByQuarter>> GetUserCreationByQuarterAsync(int yearsBack = 5)
        {
            var startDate = DateTime.UtcNow.AddYears(-yearsBack);
            return await _context.Users
                .Where(u => !u.IsDeleted && u.CreatedAt >= startDate)
                .GroupBy(u => new { Year = u.CreatedAt!.Value.Year, Quarter = (u.CreatedAt!.Value.Month - 1) / 3 + 1 })
                .Select(g => new UserCreationByQuarter(
                    g.Key.Year,
                    g.Key.Quarter,
                    g.Count()
                ))
                .OrderBy(x => x.Year).ThenBy(x => x.Quarter)
                .ToListAsync();
        }
    }
}