using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class JoinedSubjectRepository : GenericRepository<JoinedSubject>
{
    public JoinedSubjectRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<(object joinedSubjects, int totalCount)> GetAllBySelfLatestSemesterPagedAsync(
     int pageNumber,
     int pageSize,
     long studentProfileId)
    {
        var latestSemesterId = await _context.JoinedSubjects
            .Where(js => js.StudentProfileId == studentProfileId)
            .OrderByDescending(js => js.Semester.CreatedAt)
            .Select(js => js.SemesterId)
            .FirstOrDefaultAsync();

        if (latestSemesterId == 0)
            return (Array.Empty<JoinedSubject>(), 0);

        var query = _context.JoinedSubjects
            .Where(js => js.StudentProfileId == studentProfileId && js.SemesterId == latestSemesterId);

        var totalCount = await query.CountAsync();
        var joinedSubjects = await query
            .OrderByDescending(js => js.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (joinedSubjects, totalCount);
    }


    public async Task<(object joinedSubjects, int totalCount)> GetAllByStudentProfileIDPagedAsync(int pageNumber, int pageSize, long studentProfileId)
    {
        var query = _context.JoinedSubjects.Where(js => js.StudentProfileId == studentProfileId);

        var totalCount = await query.CountAsync();

        var joinedSubjects = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (joinedSubjects, totalCount);
    }

    public async Task<JoinedSubject> GetByIdWStudentUserIdAsync(long id)
    {
        return await _context.JoinedSubjects
            .Include(js => js.StudentProfile)
            .FirstOrDefaultAsync(js => js.Id == id);
    }
}