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

    public async Task<IEnumerable<JoinedSubject>> GetAllByStudentProfileIDAsync(long studentProfileId)
    {
        return await _context.JoinedSubjects.Where(js => js.StudentProfileId == studentProfileId).ToListAsync();
    }

    public async Task<IEnumerable<JoinedSubject>> GetAllActiveByStudentProfileIDAsync(long studentProfileId)
    {
        return await _context.JoinedSubjects.Where(js => js.StudentProfileId == studentProfileId && js.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<JoinedSubject>> GetAllActiveByStudentProfileIDLatestSemesAsync(long studentProfileId)
    {
        var latestSemesterId = await _context.JoinedSubjects
           .Where(js => js.StudentProfileId == studentProfileId)
           .OrderByDescending(js => js.Semester.CreatedAt)
           .Select(js => js.SemesterId)
           .FirstOrDefaultAsync();

        if (latestSemesterId == 0)
            return new List<JoinedSubject>();



        return await _context.JoinedSubjects.Where(js => js.StudentProfileId == studentProfileId
        && js.IsActive
        && js.SemesterId == latestSemesterId).ToListAsync();
    }

    public async Task<JoinedSubject> GetByIdWStudentProfileAsync(long id)
    {
        return await _context.JoinedSubjects
            .Include(js => js.StudentProfile)
            .FirstOrDefaultAsync(js => js.Id == id);
    }
}