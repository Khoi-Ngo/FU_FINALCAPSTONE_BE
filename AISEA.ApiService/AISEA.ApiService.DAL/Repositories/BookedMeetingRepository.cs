using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class BookedMeetingRepository : GenericRepository<BookedMeeting>
{
    public BookedMeetingRepository(AiseaContext context) : base(context)
    {
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
}