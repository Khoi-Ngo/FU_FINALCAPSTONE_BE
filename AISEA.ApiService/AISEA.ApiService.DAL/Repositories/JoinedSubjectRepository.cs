using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AISEA.ApiService.DAL.Repositories;

public class JoinedSubjectRepository : GenericRepository<JoinedSubject>
{
    public JoinedSubjectRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<IEnumerable<JoinedSubject>> GetAllByStudentProfileIDAsync(long studentProfileId)
    {
        return await _context.JoinedSubjects.Include(js => js.Semester).Where(js => js.StudentProfileId == studentProfileId).ToListAsync();
    }

    public async Task<IEnumerable<JoinedSubject>> GetAllActiveByStudentProfileIDWithSemesteDataAsync(long studentProfileId)
    {
        return await _context.JoinedSubjects.Include(js => js.Semester).Where(js => js.StudentProfileId == studentProfileId && js.IsActive).ToListAsync();
    }

    public async Task<(JoinedSubject removedJoinedSubject, IEnumerable<JoinedSubject> otherJoinedSubjects)> GetByIdToRemoveAsync(long id)
    {
        var removedJoinedSubject = await _context.JoinedSubjects

            .Include(js => js.StudentProfile)
            .Include(js => js.SubjectMarkReports)
            .FirstOrDefaultAsync(js => js.Id == id);

        var otherJoinedSubjects = await _context.JoinedSubjects
                    .Where(js => js.StudentProfileId == removedJoinedSubject.StudentProfileId
                    && js.Id != id
                    && js.IsActive).ToListAsync();
        return (removedJoinedSubject, otherJoinedSubjects);
    }

    public async Task RemoveAllNonUseAsync()
    {
        var latestSemesterId = await _context.Semesters
       .OrderByDescending(s => s.Id)
       .Select(s => s.Id)
       .FirstOrDefaultAsync();

        if (latestSemesterId == 0)
            return;

        int totalDeleted = 0;

        while (true)
        {
            var oldSubjects = await _context.JoinedSubjects
                .Where(js => js.SemesterId < latestSemesterId && !js.SubjectMarkReports.Any())
                .OrderBy(js => js.Id)
                .ToListAsync();

            if (oldSubjects.Count == 0)
                break; // No more to delete

            _context.JoinedSubjects.RemoveRange(oldSubjects);
            totalDeleted += oldSubjects.Count;

            await _context.SaveChangesAsync();
        }

        Console.WriteLine($"{DateTime.UtcNow} Removed {totalDeleted} non-use joined subjects.");

    }


    public async Task BulkUpdateAsync(IEnumerable<JoinedSubject> subjects)
    {
        _context.JoinedSubjects.UpdateRange(subjects);
        await _context.SaveChangesAsync();
    }


    public async Task<IEnumerable<JoinedSubject>> GetAllByStudentProfileIDNoSemesterAsync(long studentProfileId)
    {
        return await _context.JoinedSubjects.Where(js => js.StudentProfileId == studentProfileId).ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllPassedSubjectCodesAsync(long studentProfileId)
    {
        return await _context.JoinedSubjects.Where(js => js.StudentProfileId == studentProfileId
            && js.IsPassed
        ).Select(js => js.SubjectCode).ToListAsync();
    }
}