using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class JoinedSubjectCheckPointRepository : GenericRepository<JoinedSubjectCheckPoint>
{
    public JoinedSubjectCheckPointRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<JoinedSubjectCheckPoint> checkpoints, int totalCount)>
        GetAllByStudentProfileIdAsync(
            long studentProfileId,
            bool isInCompletedOnly,
            bool isOrderedByNearToFarDeadline,
            bool isActiveOnly,
            PaginationRequest paginationRequest)
    {
        var baseQuery = _context.JoinedSubjects
            .Where(js => js.StudentProfileId == studentProfileId);

        // filter by active subjects if required
        if (isActiveOnly)
        {
            baseQuery = baseQuery.Where(js => js.IsActive);
        }

        var query = baseQuery.SelectMany(js => js.JoinedSubjectCheckPoints);

        // filter not completed only
        if (isInCompletedOnly)
        {
            query = query.Where(jsc => !jsc.IsCompleted);
        }

        // apply ordering
        query = isOrderedByNearToFarDeadline
            ? query.OrderBy(jsc => jsc.Deadline)
            : query.OrderByDescending(jsc => jsc.Deadline);

        // total count before paging
        var totalCount = await query.CountAsync();

        // apply pagination
        var checkpoints = await query
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync();

        return (checkpoints, totalCount);
    }


    public async Task<List<JoinedSubjectCheckPoint>> GetAllByStuProfileIdUpcomingAsync(long studentProfileId, int limit)
    {
        return await _context.JoinedSubjects
            .Where(js => js.IsActive && js.StudentProfileId == studentProfileId)
            .SelectMany(js => js.JoinedSubjectCheckPoints)
            .Where(jsc => jsc.Deadline > DateTime.Now)
            .OrderBy(jsc => jsc.Deadline)
            .Take(limit)
            .ToListAsync();
    }


    public async Task<JoinedSubjectCheckPoint> GetByIdWithJoinedSubjectAsync(long id)
    => await _context.JoinedSubjectCheckPoints.Include(c => c.JoinedSubject).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<JoinedSubjectCheckPoint>> GetByJoinedSubjectIdAsync(long joinedSubjectId)
    => await _context.JoinedSubjectCheckPoints.Where(jsc => jsc.JoinedSubjectId == joinedSubjectId).ToListAsync();

    public async Task RemoveByJoinedSubjectIdAsync(long joinedSubjectId)
    {
        var checkpoints = await _context.JoinedSubjectCheckPoints
            .Where(jsc => jsc.JoinedSubjectId == joinedSubjectId)
            .ToListAsync();

        if (checkpoints.Any())
        {
            _context.JoinedSubjectCheckPoints.RemoveRange(checkpoints);
            await _context.SaveChangesAsync();
        }
    }

}