using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
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

    public async Task<(JoinedSubject? joinedSubject, long? syllabusId)> GetJoinedSubjectWithSyllabusIdAsync(long joinedSubjectId, long studentProfileId)
    {
        var joinedSubject = await _context.JoinedSubjects
            .FirstOrDefaultAsync(js => js.Id == joinedSubjectId && js.StudentProfileId == studentProfileId);

        if (joinedSubject == null)
            return (null, null);

        // Find the syllabus ID based on SubjectCode and SubjectVersionCode
        var syllabusId = await _context.Subjects
            .Where(s => s.SubjectCode == joinedSubject.SubjectCode)
            .SelectMany(s => s.SubjectVersions)
            .Where(sv => sv.VersionCode == joinedSubject.SubjectVersionCode)
            .SelectMany(sv => sv.Syllabi)
            .Where(sy => !sy.IsDeleted)
            .Select(sy => sy.Id)
            .FirstOrDefaultAsync();

        return (joinedSubject, syllabusId == 0 ? null : syllabusId);
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

    public async Task<List<JoinedSubjectStatusDto>> GetMapJoinedSubjectStatusByStudentProfileIDAsync(long studentProfileID)
    {
        var query = from js in _context.JoinedSubjects
                    where js.StudentProfileId == studentProfileID
                    select new
                    {
                        js.Id,
                        js.IsPassed,
                        TotalWeight = js.SubjectMarkReports.Sum(r => (double?)r.Weight) ?? 0,
                        AvgScore = js.SubjectMarkReports.Any() ? js.SubjectMarkReports.Average(r => (double?)r.Score) ?? 0 : 0,
                        AnyBelowMin = js.SubjectMarkReports.Any(r => r.Score < r.MinScore),
                        HasReports = js.SubjectMarkReports.Any()
                    };

        var data = await query.ToListAsync();

        var result = new List<JoinedSubjectStatusDto>();

        foreach (var js in data)
        {
            string status;

            if (js.IsPassed)
                status = "PASSED";
            else if (!js.HasReports)
                status = "IN-PROGRESS";
            else if (Math.Abs(js.TotalWeight - 100) > 0.001)
                status = "IN-PROGRESS";
            else
                status = (js.AvgScore >= 5 && !js.AnyBelowMin) ? "PASSED" : "NOT PASSED";

            result.Add(new JoinedSubjectStatusDto
            {
                JoinedSubjectId = js.Id,
                Status = status
            });
        }

        return result;
    }

    public async Task<List<JoinedSubjectCheckpointProgressDto>> GetMapJoinedSubjectProgressCheckpointByStudentProfileIDAsync(long studentProfileID)
    {
        var data = await _context.JoinedSubjects
            .Where(js => js.StudentProfileId == studentProfileID)
            .Select(js => new
            {
                js.Id,
                TotalCheckpoints = js.JoinedSubjectCheckPoints.Count(),
                CompletedCheckpoints = js.JoinedSubjectCheckPoints.Count(cp => cp.IsCompleted)
            })
            .ToListAsync();

        var result = data.Select(js => new JoinedSubjectCheckpointProgressDto
        {
            JoinedSubjectId = js.Id,
            CompletedPercentage = js.TotalCheckpoints == 0
                ? 0
                : Math.Round((js.CompletedCheckpoints * 100.0) / js.TotalCheckpoints, 2)
        }).ToList();

        return result;
    }

}
