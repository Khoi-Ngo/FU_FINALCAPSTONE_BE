using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class BookingAvaiForDashboardRepo : GenericRepository<BookingAvailability>
    {
        public BookingAvaiForDashboardRepo(AiseaContext context) : base(context)
        {
        }

        // DTOs for return values
        public record AvailabilityByDay(DayOfWeekAISEA Day, int SlotCount);
        public record StaffAvailabilityHours(string StaffName, double TotalHours);
        public record DepartmentAvailability(string Department, int TotalSlots);
        public record CampusAvailabilityDistribution(string Campus, int SlotCount);
        public record AvailabilityTrend(DateTime Month, int SlotCount);
        public record StaffAvailabilityDetails(string StaffName, DayOfWeekAISEA Day, TimeSpan StartTime, TimeSpan EndTime);
        public record AvailabilityByPosition(string Position, int TotalSlots);
        public record StaffAvailabilitySummary(string StaffName, int TotalSlots, double AvgHoursPerSlot);
        public record AvailabilityByTimeSlot(TimeSpan StartTime, int SlotCount);
        public record StaffAvailabilityStatus(string StaffName, bool IsActive, int SlotCount);
        public record DepartmentWorkload(string Department, int StaffCount, int TotalSlots);
        public record AvailabilityByQuarter(int Year, int Quarter, int SlotCount);
        public record StaffProfileCompleteness(string StaffName, int ProfileCompleteness, int SlotCount);
        public record CampusDepartmentAvailability(string Campus, string Department, int SlotCount);
        public record StaffAvailabilityByWeek(string StaffName, int WeekNumber, int SlotCount);

        // 1. Availability slots by day of week for pie chart (admin: all staff, staff: own data)
        public async Task<List<AvailabilityByDay>> GetAvailabilityByDayAsync(long? staffProfileId = null)
        {
            var query = _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted);

            if (staffProfileId.HasValue)
                query = query.Where(ba => ba.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ba => ba.DayInWeek)
                .Select(g => new AvailabilityByDay(
                    g.Key,
                    g.Count()
                ))
                .OrderBy(x => x.Day)
                .ToListAsync();
        }

        // 2. Total availability hours per staff for bar chart (admin: all staff, staff: own data)
        public async Task<List<StaffAvailabilityHours>> GetStaffAvailabilityHoursAsync(long? staffProfileId = null)
        {
            var query = _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted);

            if (staffProfileId.HasValue)
                query = query.Where(ba => ba.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ba => ba.StaffProfile!.User.FirstName + " " + ba.StaffProfile.User.LastName)
                .Select(g => new StaffAvailabilityHours(
                    g.Key,
                    g.Sum(ba => (ba.EndTime - ba.StartTime).TotalHours)
                ))
                .OrderByDescending(x => x.TotalHours)
                .ToListAsync();
        }

        // 3. Availability slots by department for pie chart (admin only)
        public async Task<List<DepartmentAvailability>> GetAvailabilityByDepartmentAsync()
        {
            return await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => ba.StaffProfile!.Department)
                .Select(g => new DepartmentAvailability(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.TotalSlots)
                .ToListAsync();
        }

        // 4. Availability slots by campus for bar chart (admin only)
        public async Task<List<CampusAvailabilityDistribution>> GetAvailabilityByCampusAsync()
        {
            return await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => ba.StaffProfile!.Campus)
                .Select(g => new CampusAvailabilityDistribution(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.SlotCount)
                .ToListAsync();
        }

        // 5. Availability trend over time for line chart (admin: all staff, staff: own data)
        public async Task<List<AvailabilityTrend>> GetAvailabilityTrendAsync(int monthsBack = 12, long? staffProfileId = null)
        {
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack);
            var query = _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted && ba.CreatedAt >= startDate);

            if (staffProfileId.HasValue)
                query = query.Where(ba => ba.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ba => new { Year = ba.CreatedAt!.Value.Year, Month = ba.CreatedAt!.Value.Month })
                .Select(g => new AvailabilityTrend(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Count()
                ))
                .OrderBy(x => x.Month)
                .ToListAsync();
        }

        // 6. Staff availability details for table (admin: all staff, staff: own data)
        public async Task<List<StaffAvailabilityDetails>> GetStaffAvailabilityDetailsAsync(long? staffProfileId = null)
        {
            var query = _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted);

            if (staffProfileId.HasValue)
                query = query.Where(ba => ba.StaffProfileId == staffProfileId.Value);

            return await query
                .Select(ba => new StaffAvailabilityDetails(
                    ba.StaffProfile!.User.FirstName + " " + ba.StaffProfile.User.LastName,
                    ba.DayInWeek,
                    ba.StartTime,
                    ba.EndTime
                ))
                .OrderBy(x => x.StaffName)
                .ThenBy(x => x.Day)
                .ThenBy(x => x.StartTime)
                .Take(10)
                .ToListAsync();
        }

        // 7. Availability slots by position for bar chart (admin only)
        public async Task<List<AvailabilityByPosition>> GetAvailabilityByPositionAsync()
        {
            return await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => ba.StaffProfile!.Position)
                .Select(g => new AvailabilityByPosition(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.TotalSlots)
                .ToListAsync();
        }

        // 8. Staff availability summary for table (admin: all staff, staff: own data)
        public async Task<List<StaffAvailabilitySummary>> GetStaffAvailabilitySummaryAsync(long? staffProfileId = null)
        {
            var query = _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted);

            if (staffProfileId.HasValue)
                query = query.Where(ba => ba.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ba => ba.StaffProfile!.User.FirstName + " " + ba.StaffProfile.User.LastName)
                .Select(g => new StaffAvailabilitySummary(
                    g.Key,
                    g.Count(),
                    g.Average(ba => (ba.EndTime - ba.StartTime).TotalHours)
                ))
                .OrderByDescending(x => x.TotalSlots)
                .ToListAsync();
        }

        // 9. Availability by time slot for bar chart (admin: all staff, staff: own data)
        public async Task<List<AvailabilityByTimeSlot>> GetAvailabilityByTimeSlotAsync(long? staffProfileId = null)
        {
            var query = _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted);

            if (staffProfileId.HasValue)
                query = query.Where(ba => ba.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ba => ba.StartTime)
                .Select(g => new AvailabilityByTimeSlot(
                    g.Key,
                    g.Count()
                ))
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

        // 10. Staff availability status for table (admin: all staff, staff: own data)
        public async Task<List<StaffAvailabilityStatus>> GetStaffAvailabilityStatusAsync(long? staffProfileId = null)
        {
            var query = _context.StaffProfiles
                .Where(sp => !sp.IsDeleted);

            if (staffProfileId.HasValue)
                query = query.Where(sp => sp.Id == staffProfileId.Value);

            return await query
                .Select(sp => new StaffAvailabilityStatus(
                    sp.User.FirstName + " " + sp.User.LastName,
                    sp.EndWorkAt == null || sp.EndWorkAt > DateTimeOffset.UtcNow,
                    sp.BookingAvailabilities.Count()
                ))
                .OrderByDescending(x => x.SlotCount)
                .ToListAsync();
        }

        // 11. Department workload by staff and slots for table (admin only)
        public async Task<List<DepartmentWorkload>> GetDepartmentWorkloadAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted)
                .GroupBy(sp => sp.Department)
                .Select(g => new DepartmentWorkload(
                    g.Key,
                    g.Count(),
                    g.Sum(sp => sp.BookingAvailabilities.Count())
                ))
                .OrderByDescending(x => x.TotalSlots)
                .ToListAsync();
        }

        // 12. Availability by quarter for line chart (admin: all staff, staff: own data)
        public async Task<List<AvailabilityByQuarter>> GetAvailabilityByQuarterAsync(int yearsBack = 5, long? staffProfileId = null)
        {
            var startDate = DateTime.UtcNow.AddYears(-yearsBack);
            var query = _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted && ba.CreatedAt >= startDate);

            if (staffProfileId.HasValue)
                query = query.Where(ba => ba.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(ba => new { Year = ba.CreatedAt!.Value.Year, Quarter = (ba.CreatedAt!.Value.Month - 1) / 3 + 1 })
                .Select(g => new AvailabilityByQuarter(
                    g.Key.Year,
                    g.Key.Quarter,
                    g.Count()
                ))
                .OrderBy(x => x.Year).ThenBy(x => x.Quarter)
                .ToListAsync();
        }

        // 13. Staff profile completeness for table (admin: all staff, staff: own data)
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
                    sp.BookingAvailabilities.Count()
                ))
                .OrderByDescending(x => x.ProfileCompleteness)
                .ToListAsync();
        }

        // 14. Availability by campus and department for stacked bar chart (admin only)
        public async Task<List<CampusDepartmentAvailability>> GetCampusDepartmentAvailabilityAsync()
        {
            return await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => new { ba.StaffProfile!.Campus, ba.StaffProfile!.Department })
                .Select(g => new CampusDepartmentAvailability(
                    g.Key.Campus,
                    g.Key.Department,
                    g.Count()
                ))
                .OrderBy(x => x.Campus).ThenBy(x => x.Department)
                .ToListAsync();
        }

    
    
    }
}