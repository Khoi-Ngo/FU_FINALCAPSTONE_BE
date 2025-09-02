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
        public MeetingForDashboardRepo(AiseaContext context) : base(context)
        {
        }

        // DTOs for return values
        public record MeetingByStatus(EBookingStatus Status, int MeetingCount);
        public record StaffMeetingLoad(string StaffName, int MeetingCount);
        public record StudentMeetingParticipation(string StudentName, int MeetingCount);
        public record MeetingTrend(DateTime Month, int MeetingCount);
        public record MeetingDetails(string StaffName, string StudentName, DateTime StartDateTime, EBookingStatus Status, string TitleStudentIssue);
        public record MeetingByDepartment(string Department, int MeetingCount);
        public record MeetingByCampus(string Campus, int MeetingCount);
        public record MeetingDurationByStaff(string StaffName, double TotalHours);
        public record MeetingByDayOfWeek(DayOfWeek Day, int MeetingCount);
        public record MeetingFeedbackSummary(string StaffName, int MeetingsWithFeedback, int TotalMeetings);
        public record DepartmentMeetingWorkload(string Department, int StaffCount, int MeetingCount);
        public record MeetingByQuarter(int Year, int Quarter, int MeetingCount);
        public record StaffProfileCompleteness(string StaffName, int ProfileCompleteness, int MeetingCount);
        public record CampusDepartmentMeetings(string Campus, string Department, int MeetingCount);
        public record StudentIssueFrequency(string IssueTitle, int MeetingCount);

        // 1. Meetings by status for pie chart (admin only)
        public async Task<List<MeetingByStatus>> GetMeetingsByStatusAsync()
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => bm.Status)
                .Select(g => new MeetingByStatus(
                    g.Key,
                    g.Count()
                ))
                .OrderBy(x => x.Status)
                .ToListAsync();
        }

        // 2. Meeting load per staff for bar chart (admin: all staff, staff: own data)
        public async Task<List<StaffMeetingLoad>> GetStaffMeetingLoadAsync(long? staffProfileId = null)
        {
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(bm => bm.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(bm => bm.StaffProfile!.User.FirstName + " " + bm.StaffProfile.User.LastName)
                .Select(g => new StaffMeetingLoad(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.MeetingCount)
                .ToListAsync();
        }

        // 3. Meeting participation per student for bar chart (admin: all students, student: own data)
        public async Task<List<StudentMeetingParticipation>> GetStudentMeetingParticipationAsync(long? studentProfileId = null)
        {
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted);
            
            if (studentProfileId.HasValue)
                query = query.Where(bm => bm.StudentProfileId == studentProfileId.Value);

            return await query
                .GroupBy(bm => bm.StudentProfile!.User.FirstName + " " + bm.StudentProfile.User.LastName)
                .Select(g => new StudentMeetingParticipation(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.MeetingCount)
                .ToListAsync();
        }

        // 4. Meeting trend over time for line chart (admin: all, staff: own, student: own)
        public async Task<List<MeetingTrend>> GetMeetingTrendAsync(int monthsBack = 12, long? staffProfileId = null, long? studentProfileId = null)
        {
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack);
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted && bm.CreatedAt >= startDate);
            
            if (staffProfileId.HasValue)
                query = query.Where(bm => bm.StaffProfileId == staffProfileId.Value);
            if (studentProfileId.HasValue)
                query = query.Where(bm => bm.StudentProfileId == studentProfileId.Value);

            return await query
                .GroupBy(bm => new { Year = bm.CreatedAt!.Value.Year, Month = bm.CreatedAt!.Value.Month })
                .Select(g => new MeetingTrend(
                    new DateTime(g.Key.Year, g.Key.Month, 1),
                    g.Count()
                ))
                .OrderBy(x => x.Month)
                .ToListAsync();
        }

        // 5. Meeting details for table (admin: all, staff: own, student: own)
        public async Task<List<MeetingDetails>> GetMeetingDetailsAsync(long? staffProfileId = null, long? studentProfileId = null)
        {
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(bm => bm.StaffProfileId == staffProfileId.Value);
            if (studentProfileId.HasValue)
                query = query.Where(bm => bm.StudentProfileId == studentProfileId.Value);

            return await query
                .Select(bm => new MeetingDetails(
                    bm.StaffProfile!.User.FirstName + " " + bm.StaffProfile.User.LastName,
                    bm.StudentProfile!.User.FirstName + " " + bm.StudentProfile.User.LastName,
                    bm.StartDateTime,
                    bm.Status,
                    bm.TitleStudentIssue
                ))
                .OrderByDescending(x => x.StartDateTime)
                .Take(10)
                .ToListAsync();
        }

        // 6. Meetings by department for pie chart (admin only)
        public async Task<List<MeetingByDepartment>> GetMeetingsByDepartmentAsync()
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => bm.StaffProfile!.Department)
                .Select(g => new MeetingByDepartment(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.MeetingCount)
                .ToListAsync();
        }

        // 7. Meetings by campus for bar chart (admin only)
        public async Task<List<MeetingByCampus>> GetMeetingsByCampusAsync()
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => bm.StaffProfile!.Campus)
                .Select(g => new MeetingByCampus(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.MeetingCount)
                .ToListAsync();
        }

        // 8. Meeting duration by staff for bar chart (admin: all staff, staff: own data)
        public async Task<List<MeetingDurationByStaff>> GetMeetingDurationByStaffAsync(long? staffProfileId = null)
        {
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(bm => bm.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(bm => bm.StaffProfile!.User.FirstName + " " + bm.StaffProfile.User.LastName)
                .Select(g => new MeetingDurationByStaff(
                    g.Key,
                    g.Sum(bm => (bm.EndDateTime - bm.StartDateTime).TotalHours)
                ))
                .OrderByDescending(x => x.TotalHours)
                .ToListAsync();
        }

        // 9. Meetings by day of week for bar chart (admin: all, staff: own, student: own)
        public async Task<List<MeetingByDayOfWeek>> GetMeetingsByDayOfWeekAsync(long? staffProfileId = null, long? studentProfileId = null)
        {
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(bm => bm.StaffProfileId == staffProfileId.Value);
            if (studentProfileId.HasValue)
                query = query.Where(bm => bm.StudentProfileId == studentProfileId.Value);

            return await query
                .GroupBy(bm => bm.StartDateTime.DayOfWeek)
                .Select(g => new MeetingByDayOfWeek(
                    g.Key,
                    g.Count()
                ))
                .OrderBy(x => x.Day)
                .ToListAsync();
        }

        // 10. Meeting feedback summary for table (admin: all staff, staff: own data)
        public async Task<List<MeetingFeedbackSummary>> GetMeetingFeedbackSummaryAsync(long? staffProfileId = null)
        {
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted);
            
            if (staffProfileId.HasValue)
                query = query.Where(bm => bm.StaffProfileId == staffProfileId.Value);

            return await query
                .GroupBy(bm => bm.StaffProfile!.User.FirstName + " " + bm.StaffProfile.User.LastName)
                .Select(g => new MeetingFeedbackSummary(
                    g.Key,
                    g.Count(bm => bm.Feedback != null),
                    g.Count()
                ))
                .OrderByDescending(x => x.TotalMeetings)
                .ToListAsync();
        }

        // 11. Department meeting workload for table (admin only)
        public async Task<List<DepartmentMeetingWorkload>> GetDepartmentMeetingWorkloadAsync()
        {
            return await _context.StaffProfiles
                .Where(sp => !sp.IsDeleted)
                .GroupBy(sp => sp.Department)
                .Select(g => new DepartmentMeetingWorkload(
                    g.Key,
                    g.Count(),
                    g.Sum(sp => sp.BookedMeetings.Count(bm => bm.StudentProfile != null && !bm.StudentProfile.IsDeleted))
                ))
                .OrderByDescending(x => x.MeetingCount)
                .ToListAsync();
        }

        // 12. Meetings by quarter for line chart (admin: all, staff: own, student: own)
        public async Task<List<MeetingByQuarter>> GetMeetingsByQuarterAsync(int yearsBack = 5, long? staffProfileId = null, long? studentProfileId = null)
        {
            var startDate = DateTime.UtcNow.AddYears(-yearsBack);
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted && bm.CreatedAt >= startDate);
            
            if (staffProfileId.HasValue)
                query = query.Where(bm => bm.StaffProfileId == staffProfileId.Value);
            if (studentProfileId.HasValue)
                query = query.Where(bm => bm.StudentProfileId == studentProfileId.Value);

            return await query
                .GroupBy(bm => new { Year = bm.CreatedAt!.Value.Year, Quarter = (bm.CreatedAt!.Value.Month - 1) / 3 + 1 })
                .Select(g => new MeetingByQuarter(
                    g.Key.Year,
                    g.Key.Quarter,
                    g.Count()
                ))
                .OrderBy(x => x.Year).ThenBy(x => x.Quarter)
                .ToListAsync();
        }

        // 13. Staff profile completeness with meeting count for table (admin: all staff, staff: own data)
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
                    sp.BookedMeetings.Count(bm => bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                ))
                .OrderByDescending(x => x.ProfileCompleteness)
                .ToListAsync();
        }

        // 14. Meetings by campus and department for stacked bar chart (admin only)
        public async Task<List<CampusDepartmentMeetings>> GetCampusDepartmentMeetingsAsync()
        {
            return await _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted)
                .GroupBy(bm => new { bm.StaffProfile!.Campus, bm.StaffProfile!.Department })
                .Select(g => new CampusDepartmentMeetings(
                    g.Key.Campus,
                    g.Key.Department,
                    g.Count()
                ))
                .OrderBy(x => x.Campus).ThenBy(x => x.Department)
                .ToListAsync();
        }

        // 15. Student issue frequency for table (admin: all, student: own data)
        public async Task<List<StudentIssueFrequency>> GetStudentIssueFrequencyAsync(long? studentProfileId = null)
        {
            var query = _context.BookedMeetings
                .Where(bm => bm.StaffProfile != null && !bm.StaffProfile.IsDeleted && bm.StudentProfile != null && !bm.StudentProfile.IsDeleted);
            
            if (studentProfileId.HasValue)
                query = query.Where(bm => bm.StudentProfileId == studentProfileId.Value);

            return await query
                .GroupBy(bm => bm.TitleStudentIssue)
                .Select(g => new StudentIssueFrequency(
                    g.Key,
                    g.Count()
                ))
                .OrderByDescending(x => x.MeetingCount)
                .Take(10)
                .ToListAsync();
        }
    }
}