using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class LeaveScheduleRepository : GenericRepository<LeaveSchedule>
{
    public LeaveScheduleRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<PagedResult<LeaveSchedule>> GetPagedAsync(PaginationRequest request)
    {
        var query = _context.LeaveSchedules
           .Include(x => x.StaffProfile);
        var totalCount = await query.CountAsync();
        var leaveSchedules = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return new PagedResult<LeaveSchedule>
        {
            Items = leaveSchedules,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<LeaveSchedule>> GetPagedAsync(PaginationRequest request, long staffProfileId)
    {
        var query = _context.LeaveSchedules
         .Include(x => x.StaffProfile)
         .Where(x => x.StaffProfileId == staffProfileId);
        var totalCount = await query.CountAsync();
        var leaveSchedules = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return new PagedResult<LeaveSchedule>
        {
            Items = leaveSchedules,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<LeaveSchedule>> GetUpcomingPagedAsync(PaginationRequest request)
    {
        var query = _context.LeaveSchedules
           .Include(x => x.StaffProfile)
           .Where(x => x.StartDateTime >= DateTime.UtcNow);
        var totalCount = await query.CountAsync();
        var leaveSchedules = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return new PagedResult<LeaveSchedule>
        {
            Items = leaveSchedules,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<LeaveSchedule>> GetUpcomingPagedAsync(PaginationRequest request, long staffProfileId)
    {
        var query = _context.LeaveSchedules
             .Include(x => x.StaffProfile)
             .Where(x => x.StartDateTime >= DateTime.UtcNow
             && x.StaffProfileId == staffProfileId);
        var totalCount = await query.CountAsync();
        var leaveSchedules = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return new PagedResult<LeaveSchedule>
        {
            Items = leaveSchedules,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}