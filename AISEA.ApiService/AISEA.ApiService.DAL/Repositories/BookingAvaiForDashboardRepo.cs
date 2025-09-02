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

        // DTOs
        public record AvailabilityByDay(DayOfWeekAISEA Day, int SlotCount);
        public record StaffAvailabilityHours(string StaffName, double TotalHours);
        public record DepartmentAvailability(string Department, int TotalSlots);
        public record CampusAvailabilityDistribution(string Campus, int SlotCount);

        // ------------------------
        // STAFF METHODS
        // ------------------------

        // 1. Staff availability slots by day (pie chart)
        public async Task<List<AvailabilityByDay>> GetStaffAvailabilityByDayAsync(long staffProfileId)
        {
            return await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfileId == staffProfileId &&
                             ba.StaffProfile != null &&
                             !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => ba.DayInWeek)
                .Select(g => new AvailabilityByDay(g.Key, g.Count()))
                .OrderBy(x => x.Day)
                .ToListAsync();
        }

        // 2. Staff total availability hours (bar chart)
        public async Task<List<StaffAvailabilityHours>> GetStaffAvailabilityHoursAsync(long staffProfileId)
        {
            var data = await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfileId == staffProfileId &&
                             ba.StaffProfile != null &&
                             !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => new { ba.StaffProfileId, ba.StaffProfile!.User.FirstName, ba.StaffProfile.User.LastName })
                .Select(g => new
                {
                    g.Key.FirstName,
                    g.Key.LastName,
                    TotalHours = g.Sum(ba => (ba.EndTime - ba.StartTime).TotalHours)
                })
                .ToListAsync();

            return data.Select(x => new StaffAvailabilityHours(
                $"{x.FirstName} {x.LastName}",
                x.TotalHours
            )).ToList();
        }

        // ------------------------
        // ADMIN METHODS
        // ------------------------

        // 3. Availability slots by department (pie chart)
        public async Task<List<DepartmentAvailability>> GetAvailabilityByDepartmentAsync()
        {
            return await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => ba.StaffProfile!.Department)
                .Select(g => new DepartmentAvailability(g.Key, g.Count()))
                .OrderByDescending(x => x.TotalSlots)
                .ToListAsync();
        }

        // 4. Availability slots by campus (bar chart)
        public async Task<List<CampusAvailabilityDistribution>> GetAvailabilityByCampusAsync()
        {
            return await _context.BookingAvailabilities
                .Where(ba => ba.StaffProfile != null && !ba.StaffProfile.IsDeleted)
                .GroupBy(ba => ba.StaffProfile!.Campus)
                .Select(g => new CampusAvailabilityDistribution(g.Key, g.Count()))
                .OrderByDescending(x => x.SlotCount)
                .ToListAsync();
        }
    }
}