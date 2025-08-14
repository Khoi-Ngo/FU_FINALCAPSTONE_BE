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

    public async Task RemoveAllNonUseAsync()
    {
        var latestSemesterId = await _context.Semesters
       .OrderByDescending(s => s.Id)
       .Select(s => s.Id)
       .FirstOrDefaultAsync();

        if (latestSemesterId == 0)
            return;

        const int batchSize = 5000; // Safe limit per deletion
        int totalDeleted = 0;

        while (true)
        {
            var oldSubjects = await _context.JoinedSubjects
                .Where(js => js.SemesterId < latestSemesterId && !js.IsCompleted)
                .OrderBy(js => js.Id)
                .Take(batchSize)
                .ToListAsync();

            if (oldSubjects.Count == 0)
                break; // No more to delete

            _context.JoinedSubjects.RemoveRange(oldSubjects);
            totalDeleted += oldSubjects.Count;

            await _context.SaveChangesAsync();
        }

        Console.WriteLine($"{DateTime.UtcNow} Removed {totalDeleted} non-use joined subjects.");

    }
}