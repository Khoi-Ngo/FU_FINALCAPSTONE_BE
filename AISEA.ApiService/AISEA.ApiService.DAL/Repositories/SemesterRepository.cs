using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.CourseTrack;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class SemesterRepository : GenericRepository<Semester>
{
    public SemesterRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<PagedResult<SemesterReferDTO>> GetAllAsyncPaged(PaginationRequest request)
    {
        var query = _context.Semesters
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SemesterReferDTO
            {
                Id = s.Id,
                SemesterName = s.SemesterName
            });

        var totalCount = await query.CountAsync();
        var semesters = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResult<SemesterReferDTO>
        {
            Items = semesters,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<bool> SemesterExistsAsync(string semesterName)
    {
        return await _context.Semesters.AnyAsync(s => s.SemesterName == semesterName);
    }
}