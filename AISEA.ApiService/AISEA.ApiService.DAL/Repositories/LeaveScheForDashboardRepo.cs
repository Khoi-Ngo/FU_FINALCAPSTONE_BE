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
        public LeaveScheForDashboardRepo(AiseaContext context) : base(context) { }

        // DTOs
        public record DepartmentLeave(string Department, int TotalLeaves);
        public record CampusLeave(string Campus, int TotalLeaves);
        public record StaffLeaveDuration(string StaffName, double TotalDays);
        public record LeaveTrendPoint(string Month, int LeaveCount);

        // ------------------------
        // ADMIN METHODS
        // ------------------------

        // 1. Leave distribution by department (pie chart)
        public async Task<List<DepartmentLeave>> GetLeaveByDepartmentAsync()
        {
            return await _context.LeaveSchedules
                .Where(l => l.StaffProfile != null && !l.StaffProfile.IsDeleted)
                .GroupBy(l => l.StaffProfile!.Department)
                .Select(g => new DepartmentLeave(g.Key, g.Count()))
                .OrderByDescending(x => x.TotalLeaves)
                .ToListAsync();
        }

        // 2. Leave distribution by campus (bar chart)
        public async Task<List<CampusLeave>> GetLeaveByCampusAsync()
        {
            return await _context.LeaveSchedules
                .Where(l => l.StaffProfile != null && !l.StaffProfile.IsDeleted)
                .GroupBy(l => l.StaffProfile!.Campus)
                .Select(g => new CampusLeave(g.Key, g.Count()))
                .OrderByDescending(x => x.TotalLeaves)
                .ToListAsync();
        }

        // ------------------------
        // STAFF METHODS
        // ------------------------

        // 3. Total leave duration for a specific staff
        public async Task<StaffLeaveDuration> GetStaffLeaveDurationAsync(long staffProfileId)
        {
            var leaves = await _context.LeaveSchedules
                .Where(l => l.StaffProfileId == staffProfileId &&
                            l.StaffProfile != null &&
                            !l.StaffProfile.IsDeleted)
                .Select(l => new
                {
                    l.StartDateTime,
                    l.EndDateTime,
                    l.StaffProfile!.User.FirstName,
                    l.StaffProfile.User.LastName
                })
                .ToListAsync();

            var totalDays = leaves.Sum(l => (l.EndDateTime - l.StartDateTime).TotalDays + 1);

            var staffName = leaves.FirstOrDefault() is { } first
                ? $"{first.FirstName} {first.LastName}"
                : "Unknown Staff";

            return new StaffLeaveDuration(staffName, totalDays);
        }

        // 4. Leave trend (last X months) for a specific staff
        public async Task<List<LeaveTrendPoint>> GetLeaveTrendAsync(int monthsBack, long staffProfileId)
        {
            var cutoff = DateTime.UtcNow.AddMonths(-monthsBack);

            var leaves = await _context.LeaveSchedules
                .Where(l => l.StaffProfileId == staffProfileId &&
                            l.StaffProfile != null &&
                            !l.StaffProfile.IsDeleted &&
                            l.StartDateTime >= cutoff)
                .ToListAsync();

            var trend = leaves
                .GroupBy(l => new { l.StartDateTime.Year, l.StartDateTime.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new LeaveTrendPoint(
                    $"{g.Key.Year}-{g.Key.Month:D2}",
                    g.Count()
                ))
                .ToList();

            return trend;
        }
    }
}
