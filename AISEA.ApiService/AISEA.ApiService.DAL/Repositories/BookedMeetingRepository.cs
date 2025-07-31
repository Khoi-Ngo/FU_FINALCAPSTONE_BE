using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class BookedMeetingRepository : GenericRepository<BookedMeeting>
{
    public BookedMeetingRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<(BookedMeeting Meeting, long AdvisorUserId)> GetMeetingWithAdvisorUserIdAsync(long id)
    {
        return await _context.BookedMeetings
            .Where(m => m.Id == id)
            .Join(_context.StaffProfiles,
                m => m.StaffProfileId,
                sp => sp.Id,
                (m, sp) => new ValueTuple<BookedMeeting, long>(m, sp.UserId))
            .FirstAsync();
    }

    public async Task<(BookedMeeting Meeting, long StudentUserId)> GetMeetingWithStudentUserIdAsync(long id)
    {
        return await _context.BookedMeetings
            .Where(m => m.Id == id)
            .Join(_context.StudentProfiles,
                m => m.StudentProfileId,
                sp => sp.Id,
                (m, sp) => new ValueTuple<BookedMeeting, long>(m, sp.UserId))
            .FirstAsync();
    }

    public async Task<(IEnumerable<BookedMeeting> meetings, int TotalCount)> GetAllAsync(PaginationRequest request)
    {
        var query = _context.BookedMeetings
            .Include(m => m.StaffProfile).ThenInclude(sp => sp.User)
            .Include(m => m.StudentProfile).ThenInclude(sp => sp.User)
            .OrderByDescending(m => m.StartDateTime);
        var totalCount = await query.CountAsync();
        var meetings = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (meetings, totalCount);
    }

    public async Task<(IEnumerable<BookedMeeting> meetings, int TotalCount)> GetAllByStudentProfileIdAsync(PaginationRequest request, long studentProfileId)
    {
        var query = _context.BookedMeetings
            .Where(m => m.StudentProfileId == studentProfileId)
            .Include(m => m.StaffProfile).ThenInclude(sp => sp.User)
            .Include(m => m.StudentProfile).ThenInclude(sp => sp.User)
            .OrderByDescending(m => m.StartDateTime);
        var totalCount = await query.CountAsync();
        var meetings = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (meetings, totalCount);
    }

    public async Task<(IEnumerable<BookedMeeting> meetings, int TotalCount)> GetAllByStaffProfileIdAsync(PaginationRequest request, long staffProfileId)
    {
        var query = _context.BookedMeetings
            .Where(m => m.StaffProfileId == staffProfileId)
            .Include(m => m.StaffProfile).ThenInclude(sp => sp.User)
            .Include(m => m.StudentProfile).ThenInclude(sp => sp.User)
            .OrderByDescending(m => m.StartDateTime);
        var totalCount = await query.CountAsync();
        var meetings = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (meetings, totalCount);
    }

    public async Task<BookedMeeting> GetDetailByIdAsync(long id)
    {
        return await _context.BookedMeetings
            .Include(m => m.StaffProfile).ThenInclude(sp => sp.User)
            .Include(m => m.StudentProfile).ThenInclude(sp => sp.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }


    public async Task<List<OverdueMeetingDTO>> GetPendingOverdueMeetingsWithUserIdsAsync()
    {
        return await _context.BookedMeetings
            .Where(m => m.Status == EBookingStatus.PENDING && m.StartDateTime < DateTime.UtcNow)
            .Join(_context.StaffProfiles,
                m => m.StaffProfileId,
                sp => sp.Id,
                (m, sp) => new { Meeting = m, StaffUserId = sp.UserId })
            .Join(_context.StudentProfiles,
                ms => ms.Meeting.StudentProfileId,
                sp => sp.Id,
                (ms, sp) => new OverdueMeetingDTO
                {
                    Id = ms.Meeting.Id,
                    StaffUserId = ms.StaffUserId,
                    StudentUserId = sp.UserId,
                    StartDateTime = ms.Meeting.StartDateTime,
                    Status = ms.Meeting.Status
                })
            .ToListAsync();
    }

    public async Task UpdateMeetingStatusesAsync(List<long> meetingIds, EBookingStatus status)
    {
        await _context.BookedMeetings
            .Where(m => meetingIds.Contains(m.Id))
            .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.Status, status));
    }
    public async Task<List<StuMissedMeetingDTO>> GetConfirmedStudentMissedMeetingsAsync(int daysToCheckStudentMissedAfterEndMeeting)
    {
        return await _context.BookedMeetings
            .Where(m => m.Status == EBookingStatus.CONFIRMED &&
                        m.EndDateTime <= DateTime.UtcNow.AddDays(-daysToCheckStudentMissedAfterEndMeeting))
            .Join(_context.StudentProfiles,
                m => m.StudentProfileId,
                sp => sp.Id,
                (m, sp) => new StuMissedMeetingDTO
                {
                    Id = m.Id,
                    StudentUserId = sp.UserId,
                    StudentProfileId = m.StudentProfileId,
                    StartDateTime = m.StartDateTime
                })
            .ToListAsync();
    }

    public async Task<(IEnumerable<BookedMeeting> meetings, int TotalCount)> GetAllActiveByStaffProfileIdAsync(PaginationRequest request, long staffProfileId)
    {
        var activeStatuses = new[]
        {
        EBookingStatus.PENDING,
        EBookingStatus.CONFIRMED,
        EBookingStatus.COMPLETED,
        EBookingStatus.STUDENT_MISSED,
        EBookingStatus.ADVISOR_MISSED
    };

        var query = _context.BookedMeetings
            .Where(m => m.StaffProfileId == staffProfileId && activeStatuses.Contains(m.Status))
            .Include(m => m.StaffProfile).ThenInclude(sp => sp.User)
            .Include(m => m.StudentProfile).ThenInclude(sp => sp.User)
            .OrderByDescending(m => m.StartDateTime);

        var totalCount = await query.CountAsync();
        var meetings = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return (meetings, totalCount);
    }

    public async Task<(IEnumerable<BookedMeeting> meetings, int TotalCount)> GetAllActiveByStudentProfileIdAsync(PaginationRequest request, long studentProfileId)
    {
        var activeStatuses = new[]
        {
        EBookingStatus.PENDING,
        EBookingStatus.CONFIRMED,
        EBookingStatus.COMPLETED,
        EBookingStatus.STUDENT_MISSED,
        EBookingStatus.ADVISOR_MISSED
    };

        var query = _context.BookedMeetings
            .Where(m => m.StudentProfileId == studentProfileId && activeStatuses.Contains(m.Status))
            .Include(m => m.StaffProfile).ThenInclude(sp => sp.User)
            .Include(m => m.StudentProfile).ThenInclude(sp => sp.User)
            .OrderByDescending(m => m.StartDateTime);

        var totalCount = await query.CountAsync();
        var meetings = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return (meetings, totalCount);
    }
}