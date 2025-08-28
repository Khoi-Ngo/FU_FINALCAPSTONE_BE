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



    public new async Task<JoinedSubjectCheckPoint> GetByIdAsync(long id)
    => await _context.JoinedSubjectCheckPoints.Include(c => c.JoinedSubject).FirstOrDefaultAsync(c => c.Id == id);


    // generic helper
    public async Task<List<(long userId, string email, List<JoinedSubjectCheckPoint>)>> GetRemindAsync(
        int thresholdHours,
        string flagPropertyName)
    {
        var now = DateTime.UtcNow;
        var dueTime = now.AddHours(thresholdHours);

        var query = _context.JoinedSubjectCheckPoints
            .Where(cp =>
                !cp.IsCompleted &&
                cp.Deadline <= dueTime &&
                !EF.Property<bool>(cp, flagPropertyName))
            .Include(cp => cp.JoinedSubject)
                .ThenInclude(js => js.StudentProfile)
                .ThenInclude(sp => sp.User);

        var results = await query
            .GroupBy(cp => new
            {
                cp.JoinedSubject.StudentProfile.User.Id,
                cp.JoinedSubject.StudentProfile.User.Email
            })
            .Select(g => new
            {
                UserId = g.Key.Id,
                Email = g.Key.Email,
                Checkpoints = g.ToList()
            })
            .ToListAsync();


        return results.Select(r => (r.UserId, r.Email, r.Checkpoints)).ToList();
    }


    public async Task MarkRemind1SentAsync(IEnumerable<long> checkpointIds)
    {
        await _context.JoinedSubjectCheckPoints
            .Where(cp => checkpointIds.Contains(cp.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(cp => cp.ReminderSentHours1, true));
    }
    public async Task MarkRemind2SentAsync(IEnumerable<long> checkpointIds)
    {
        await _context.JoinedSubjectCheckPoints
            .Where(cp => checkpointIds.Contains(cp.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(cp => cp.ReminderSentHours2, true));
    }
    public async Task MarkRemind3SentAsync(IEnumerable<long> checkpointIds)
    {
        await _context.JoinedSubjectCheckPoints
            .Where(cp => checkpointIds.Contains(cp.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(cp => cp.ReminderSentHours3, true));
    }
    public async Task MarkRemind4SentAsync(IEnumerable<long> checkpointIds)
    {
        await _context.JoinedSubjectCheckPoints
            .Where(cp => checkpointIds.Contains(cp.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(cp => cp.ReminderSentHours4, true));
    }
     public async Task MarkRemind5SentAsync(IEnumerable<long> checkpointIds)
    {
        await _context.JoinedSubjectCheckPoints
            .Where(cp => checkpointIds.Contains(cp.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(cp => cp.ReminderSentHours5, true));
    }

}