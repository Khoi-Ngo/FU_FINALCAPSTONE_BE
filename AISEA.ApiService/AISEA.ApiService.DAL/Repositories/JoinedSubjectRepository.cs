using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Responses.MarkReport;
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

        if (latestSemesterId <= 1)
        {
            // No cleanup possible if we only have 0 or 1 semesters
            return;
        }
        int totalDeleted = 0;
        var cleanupThresholdSemesterId = latestSemesterId - 1;

        while (true)
        {
            var oldSubjects = await _context.JoinedSubjects
                .Where(js => js.SemesterId < cleanupThresholdSemesterId && !js.IsPassed && !js.SubjectMarkReports.Any())
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


    //check valid to conduct post comment
    public async Task<bool> IsValidToPostComment(long studentProfileId, string subjectCode)
    {
        var passedJoinedSubject = await _context.JoinedSubjects.FirstOrDefaultAsync(js => js.StudentProfileId == studentProfileId
        && js.SubjectCode == subjectCode
        && js.IsPassed);
        return passedJoinedSubject is not null;
    }

    public async Task<JoinedSubject> GetByIdWithCheckpointsAndPointsAsync(long joinedSubjectId)
    {
        return await _context.JoinedSubjects.Include(js => js.JoinedSubjectCheckPoints).Include(js => js.SubjectMarkReports).FirstOrDefaultAsync(js => js.Id == joinedSubjectId);
    }
    public async Task<List<TranscriptItemResponse>> GetTranscriptAsync(long studentProfileId)
    {
        var joinedSubjects = await _context.JoinedSubjects
            .Where(js => js.StudentProfileId == studentProfileId && js.IsActive)
            .Include(js => js.SubjectMarkReports)
            .ToListAsync();

        var transcript = joinedSubjects
            .Select(js => new TranscriptItemResponse
            {
                SubjectCode = js.SubjectCode,
                SubjectVersionCode = js.SubjectVersionCode,
                Name = js.Name,
                IsPassed = js.IsPassed,
                Credits = js.Credits,
                AvgScore = js.SubjectMarkReports.Any()
                    ? js.SubjectMarkReports.Sum(r => r.Score * r.Weight) / js.SubjectMarkReports.Sum(r => r.Weight)
                    : 0.0
            })
            .GroupBy(t => new { t.SubjectCode, t.SubjectVersionCode }) // group duplicates
            .Select(g => g.OrderByDescending(t => t.AvgScore).First()) // keep highest avg
            .ToList();

        return transcript;
    }


}