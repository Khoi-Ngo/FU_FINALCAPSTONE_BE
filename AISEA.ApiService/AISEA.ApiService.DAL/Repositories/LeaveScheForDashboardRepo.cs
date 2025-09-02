using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class LeaveScheForDashboardRepo : GenericRepository<LeaveSchedule>
    {
        public LeaveScheForDashboardRepo(AiseaContext context) : base(context)
        {
        }

        // DTOs for return values
        public record LeaveByDepartment(string Department, int LeaveCount);
        public record StaffLeaveDuration(string StaffName, double TotalDays);
        public record LeaveByCampus(string Campus, int LeaveCount);
        public record LeaveTrend(DateTime Month, int LeaveCount);
        public record StaffLeaveDetails(string StaffName, string Department, DateTime StartDate, double DurationDays);
        public record LeaveByPosition(string Position, int LeaveCount);
        public record LongLeaveStaff(string StaffName, double TotalDays, int LeaveInstances);
        public record LeaveOverlap(string Department, int OverlappingLeaves);
        public record LeaveByDayOfWeek(DayOfWeek Day, int LeaveCount);
        public record StaffLeaveStatus(string StaffName, bool IsActive, int LeaveCount);
        public record DepartmentLeaveWorkload(string Department, int StaffCount, double AvgLeaveDays);
        public record LeaveByQuarter(int Year, int Quarter, int LeaveCount);
        public record StaffProfileCompleteness(string StaffName, int ProfileCompleteness, int LeaveCount);
        public record CampusDepartmentLeave(string Campus, string Department, int LeaveCount);
        public record LeaveDurationByYear(int Year, double TotalDays);

        // 1. Leave schedules by department for pie chart (admin only)
        public async Task<List<LeaveByDepartment>> GetLeaveByDepartmentAsync()
        {
            return await _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted)
                .GroupBy(ls => ls.StaffProfile!.Department)
                .Select(g => new LeaveByDepartment(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.LeaveCount)
                .ToListAsync();
        }

        // 2. Total leave duration per staff for bar chart (admin: all staff, staff: own data)
        public async Task<List<StaffLeaveDuration>> GetStaffLeaveDurationAsync(long? staffProfileId = null)
        {
            var query = _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(ls => ls.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ls => ls.StaffProfile!.User.FirstName + " " + ls.StaffProfile.User.LastName)
                .Select(g => new StaffLeaveDuration(
                    g.Key,
                    g.Sum(ls => (ls.EndDateTime - ls.StartDateTime).TotalDays)
                ))
                .OrderByDescending(x => x.TotalDays)
                .ToListAsync();
        }

        // 3. Leave schedules by campus for bar chart (admin only)
        public async Task<List<LeaveByCampus>> GetLeaveByCampusAsync()
        {
            return await _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted)
                .GroupBy(ls => ls.StaffProfile!.Campus)
                .Select(g => new LeaveByCampus(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.LeaveCount)
                .ToListAsync();
        }

        // 4. Leave trend over time for line chart (admin: all staff, staff: own data)
        public async Task<List<LeaveTrend>> GetLeaveTrendAsync(int monthsBack = 12, long? staffProfileId = null)
        {
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack);
            var query = _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted && ls.CreatedAt >= startDate);
            
            if (staffProfileId.HasValue)
                query = query.Where(ls => ls.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ls => new { Year = ls.CreatedAt!.Value.Year, Month = ls.CreatedAt!.Value.Month })
                .Select(g => new LeaveTrend(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Count()
                ))
                .OrderBy(x => x.Month)
                .ToListAsync();
        }

        // 5. Staff leave details for table (admin: all staff, staff: own data)
        public async Task<List<StaffLeaveDetails>> GetStaffLeaveDetailsAsync(long? staffProfileId = null)
        {
            var query = _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(ls => ls.StaffProfileId == staffProfileId.Value);

            return await query
                .Select(ls => new StaffLeaveDetails(
                    ls.StaffProfile!.User.FirstName + " " + ls.StaffProfile.User.LastName,
                    ls.StaffProfile.Department,
                    ls.StartDateTime,
                    (ls.EndDateTime - ls.StartDateTime).TotalDays
                ))
                .OrderByDescending(x => x.DurationDays)
                .Take(10)
                .ToListAsync();
        }

        // 6. Leave schedules by position for pie chart (admin only)
        public async Task<List<LeaveByPosition>> GetLeaveByPositionAsync()
        {
            return await _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted)
                .GroupBy(ls => ls.StaffProfile!.Position)
                .Select(g => new LeaveByPosition(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.LeaveCount)
                .ToListAsync();
        }

        // 7. Staff with longest leave durations for table (admin: all staff, staff: own data)
        public async Task<List<LongLeaveStaff>> GetLongLeaveStaffAsync(long? staffProfileId = null)
        {
            var query = _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(ls => ls.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ls => ls.StaffProfile!.User.FirstName + " " + ls.StaffProfile.User.LastName)
                .Select(g => new LongLeaveStaff(
                    g.Key,
                    g.Sum(ls => (ls.EndDateTime - ls.StartDateTime).TotalDays),
                    g.Count()
                ))
                .OrderByDescending(x => x.TotalDays)
                .Take(10)
                .ToListAsync();
        }

        // 8. Overlapping leave schedules by department for table (admin only)
        public async Task<List<LeaveOverlap>> GetOverlappingLeavesAsync()
        {
            return await _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted)
                .GroupBy(ls => new { ls.StaffProfile!.Department, ls.StartDateTime.Date })
                .Where(g => g.Count() > 1)
                .Select(g => new LeaveOverlap(
                    g.Key.Department,
                    g.Count()
                ))
                .OrderByDescending(x => x.OverlappingLeaves)
                .ToListAsync();
        }

        // 9. Leave schedules by day of week for bar chart (admin: all staff, staff: own data)
        public async Task<List<LeaveByDayOfWeek>> GetLeaveByDayOfWeekAsync(long? staffProfileId = null)
        {
            var query = _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(ls => ls.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ls => ls.StartDateTime.DayOfWeek)
                .Select(g => new LeaveByDayOfWeek(
                    g.Key,
                    g.Count()
                ))
                .OrderBy(x => x.Day)
                .ToListAsync();
        }

        // 10. Staff leave status for table (admin: all staff, staff: own data)
        public async Task<List<StaffLeaveStatus>> GetStaffLeaveStatusAsync(long? staffProfileId = null)
        {
            var query = _context.StaffProfiles
                .Where(sp => !sp.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(sp => sp.Id == staffProfileId.Value);

            return await query
                .Select(sp => new StaffLeaveStatus(
                    sp.User.FirstName + " " + sp.User.LastName,
                    sp.EndWorkAt == null || sp.EndWorkAt > DateTimeOffset.UtcNow,
                    sp.LeaveSchedules.Count()
                ))
                .OrderByDescending(x => x.LeaveCount)
                .ToListAsync();
        }

        // 11. Department leave workload for table (admin only)
        public async Task<List<DepartmentLeaveWorkload>> GetDepartmentLeaveWorkloadAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted)
                .GroupBy(sp => sp.Department)
                .Select(g => new DepartmentLeaveWorkload(
                    g.Key,
                    g.Count(),
                    g.Average(sp => sp.LeaveSchedules.Sum(ls => (ls.EndDateTime - ls.StartDateTime).TotalDays))
                ))
                .OrderByDescending(x => x.AvgLeaveDays)
                .ToListAsync();
        }

        // 12. Leave schedules by quarter for line chart (admin: all staff, staff: own data)
        public async Task<List<LeaveByQuarter>> GetLeaveByQuarterAsync(int yearsBack = 5, long? staffProfileId = null)
        {
            var startDate = DateTime.UtcNow.AddYears(-yearsBack);
            var query = _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted && ls.CreatedAt >= startDate);
            
            if (staffProfileId.HasValue)
                query = query.Where(ls => ls.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ls => new { Year = ls.CreatedAt!.Value.Year, Quarter = (ls.CreatedAt!.Value.Month - 1) / 3 + 1 })
                .Select(g => new LeaveByQuarter(
                    g.Key.Year,
                    g.Key.Quarter,
                    g.Count()
                ))
                .OrderBy(x => x.Year).ThenBy(x => x.Quarter)
                .ToListAsync();
        }

        // 13. Staff profile completeness with leave count for table (admin: all staff, staff: own data)
        public async Task<List<StaffProfileCompleteness>> GetStaffProfileCompletenessAsync(long? staffProfileId = null)
        {
            var query = _context.StaffProfiles
                .Where(sp => !sp.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(sp => sp.Id == staffProfileId.Value);

            return await query
                .Select(sp => new StaffProfileCompleteness(
                    sp.User.FirstName + " " + sp.User.LastName,
                    (sp.Campus != null ? 25 : 0) +
                    (sp.Department != null ? 25 : 0) +
                    (sp.Position != null ? 25 : 0) +
                    (sp.StartWorkAt != null ? 25 : 0),
                    sp.LeaveSchedules.Count()
                ))
                .OrderByDescending(x => x.ProfileCompleteness)
                .ToListAsync();
        }

        // 14. Leave schedules by campus and department for stacked bar chart (admin only)
        public async Task<List<CampusDepartmentLeave>> GetCampusDepartmentLeaveAsync()
        {
            return await _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted)
                .GroupBy(ls => new { ls.StaffProfile!.Campus, ls.StaffProfile!.Department })
                .Select(g => new CampusDepartmentLeave(
                    g.Key.Campus,
                    g.Key.Department,
                    g.Count()
                ))
                .OrderBy(x => x.Campus).ThenBy(x => x.Department)
                .ToListAsync();
        }

        // 15. Total leave duration by year for line chart (admin: all staff, staff: own data)
        public async Task<List<LeaveDurationByYear>> GetLeaveDurationByYearAsync(int yearsBack = 5, long? staffProfileId = null)
        {
            var startDate = DateTime.UtcNow.AddYears(-yearsBack);
            var query = _context.LeaveSchedules
                .Where(ls => ls.StaffProfile != null && !ls.StaffProfile.IsDeleted && ls.StartDateTime >= startDate);
            
            if (staffProfileId.HasValue)
                query = query.Where(ls => ls.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ls => ls.StartDateTime.Year)
                .Select(g => new LeaveDurationByYear(
                    g.Key,
                    g.Sum(ls => (ls.EndDateTime - ls.StartDateTime).TotalDays)
                ))
                .OrderBy(x => x.Year)
                .ToListAsync();
        }
    }
}