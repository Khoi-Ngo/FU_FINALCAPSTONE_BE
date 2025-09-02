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
    public class MeetingForDashboardRepo : GenericRepository<BookedMeeting>
    {
        public MeetingForDashboardRepo(AiseaContext context) : base(context) { }

        // DTOs
        public record MeetingByStatus(EBookingStatus Status, int Count);
        public record StaffMeetingLoad(string StaffName, int Count);
        public record StudentMeetingParticipation(string StudentName, int Count);
        public record MeetingTrend(DateTime Month, int Count);
        public record MeetingDetails(string StaffName, string StudentName, DateTime Start, EBookingStatus Status, string Issue);

        // -------------------------
        // ADMIN METHODS
        // -------------------------

        // 1. Admin: Meetings by status (pie chart)
        public async Task<List<MeetingByStatus>> GetMeetingsByStatusAsync()
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted
                          && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => bm.Status)
                .Select(g => new MeetingByStatus(g.Key, g.Count()))
                .OrderBy(x => x.Status)
                .ToListAsync();
        }

        // 2. Admin: Meeting load per staff (bar chart)
        public async Task<List<StaffMeetingLoad>> GetStaffMeetingLoadAsync()
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted
                          && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => new { bm.StaffProfileId, bm.StaffProfile!.User.FirstName, bm.StaffProfile.User.LastName })
                .Select(g => new StaffMeetingLoad(
                    g.Key.FirstName + " " + g.Key.LastName,
                    g.Count()))
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        // 3. Admin: Meeting trend over time (line chart)
        public async Task<List<MeetingTrend>> GetMeetingTrendAsync(int monthsBack = 12)
        {
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack);

            return await _context.BookedMeetings
                .Where(bm => bm.CreatedAt >= startDate
                          && bm.StaffProfile != null && !bm.StaffProfile.IsDeleted
                          && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => new { bm.CreatedAt!.Value.Year, bm.CreatedAt.Value.Month })
                .Select(g => new MeetingTrend(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Count()))
                .OrderBy(x => x.Month)
                .ToListAsync();
        }

        // -------------------------
        // STAFF METHODS
        // -------------------------

        // 4. Staff: Own meeting load (bar chart)
        public async Task<List<StaffMeetingLoad>> GetOwnMeetingLoadAsync(long staffProfileId)
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StaffProfileId == staffProfileId
                          && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => new { bm.StaffProfile!.User.FirstName, bm.StaffProfile.User.LastName })
                .Select(g => new StaffMeetingLoad(
                    g.Key.FirstName + " " + g.Key.LastName,
                    g.Count()))
                .ToListAsync();
        }

  

        // -------------------------
        // STUDENT METHODS
        // -------------------------

        // 6. Student: Meeting participation (bar chart)
        public async Task<List<StudentMeetingParticipation>> GetStudentMeetingParticipationAsync(long studentProfileId)
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StudentProfileId == studentProfileId
                          && bm.StaffProfile != null && !bm.StaffProfile.IsDeleted)
                .GroupBy(bm => new { bm.StudentProfile!.User.FirstName, bm.StudentProfile.User.LastName })
                .Select(g => new StudentMeetingParticipation(
                    g.Key.FirstName + " " + g.Key.LastName,
                    g.Count()))
                .ToListAsync();
        }

  
    }

}