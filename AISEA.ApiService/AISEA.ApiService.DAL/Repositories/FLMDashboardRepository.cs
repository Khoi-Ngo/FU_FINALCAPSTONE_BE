using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Responses.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class FLMDashboardRepository
    {
        private readonly AiseaContext _context;

        public FLMDashboardRepository(AiseaContext context)
        {
            _context = context;
        }

        #region Overview Statistics

        public async Task<OverviewSummary> GetOverviewSummaryAsync()
        {
            var now = DateTime.Now;

            var totalSubjects = await _context.Subjects
                .CountAsync(s => s.ApprovalStatus == EApprovalStatus.APPROVED && !s.IsDeleted);

            var totalCurricula = await _context.Curricula
                .CountAsync(c => c.ApprovalStatus == EApprovalStatus.APPROVED && !c.IsDeleted);

            var activeSubjectVersions = await _context.SubjectVersions
                .CountAsync(sv => sv.IsActive && !sv.IsDeleted 
                    && sv.EffectiveFrom <= now 
                    && (sv.EffectiveTo == null || sv.EffectiveTo >= now));

            var totalSyllabi = await _context.Syllabi
                .CountAsync(s => s.ApprovalStatus == EApprovalStatus.APPROVED && !s.IsDeleted);

            return new OverviewSummary
            {
                TotalSubjects = totalSubjects,
                TotalCurricula = totalCurricula,
                ActiveSubjectVersions = activeSubjectVersions,
                TotalSyllabi = totalSyllabi
            };
        }

        public async Task<ApprovalStatusDistribution> GetApprovalStatusDistributionAsync()
        {
            var subjectStats = await _context.Subjects
                .Where(s => !s.IsDeleted)
                .GroupBy(s => s.ApprovalStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var curriculaStats = await _context.Curricula
                .Where(c => !c.IsDeleted)
                .GroupBy(c => c.ApprovalStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var syllabiStats = await _context.Syllabi
                .Where(s => !s.IsDeleted)
                .GroupBy(s => s.ApprovalStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return new ApprovalStatusDistribution
            {
                Subjects = new EntityApprovalStats
                {
                    Pending = subjectStats.FirstOrDefault(s => s.Status == EApprovalStatus.PENDING)?.Count ?? 0,
                    Approved = subjectStats.FirstOrDefault(s => s.Status == EApprovalStatus.APPROVED)?.Count ?? 0,
                    Rejected = subjectStats.FirstOrDefault(s => s.Status == EApprovalStatus.REJECTED)?.Count ?? 0
                },
                Curricula = new EntityApprovalStats
                {
                    Pending = curriculaStats.FirstOrDefault(s => s.Status == EApprovalStatus.PENDING)?.Count ?? 0,
                    Approved = curriculaStats.FirstOrDefault(s => s.Status == EApprovalStatus.APPROVED)?.Count ?? 0,
                    Rejected = curriculaStats.FirstOrDefault(s => s.Status == EApprovalStatus.REJECTED)?.Count ?? 0
                },
                Syllabi = new EntityApprovalStats
                {
                    Pending = syllabiStats.FirstOrDefault(s => s.Status == EApprovalStatus.PENDING)?.Count ?? 0,
                    Approved = syllabiStats.FirstOrDefault(s => s.Status == EApprovalStatus.APPROVED)?.Count ?? 0,
                    Rejected = syllabiStats.FirstOrDefault(s => s.Status == EApprovalStatus.REJECTED)?.Count ?? 0
                }
            };
        }

        #endregion

        #region Subject Statistics

        public async Task<List<SubjectsByProgramStats>> GetSubjectsByProgramAsync()
        {
            return await _context.Subjects
                .Where(s => s.ApprovalStatus == EApprovalStatus.APPROVED && !s.IsDeleted)
                .SelectMany(s => s.SubjectVersions
                    .Where(sv => !sv.IsDeleted)
                    .SelectMany(sv => sv.CurriculumSubjects
                        .Where(cs => !cs.IsDeleted)
                        .Select(cs => cs.Curriculum.Program)))
                .Where(p => !p.IsDeleted)
                .GroupBy(p => new { p.ProgramCode, p.ProgramName })
                .Select(g => new SubjectsByProgramStats
                {
                    ProgramCode = g.Key.ProgramCode,
                    ProgramName = g.Key.ProgramName,
                    SubjectCount = g.Count()
                })
                .OrderByDescending(s => s.SubjectCount)
                .ToListAsync();
        }

        public async Task<CreditDistribution> GetCreditDistributionAsync()
        {
            var subjects = await _context.Subjects
                .Where(s => s.ApprovalStatus == EApprovalStatus.APPROVED && !s.IsDeleted)
                .Select(s => s.Credits)
                .ToListAsync();

            return new CreditDistribution
            {
                OneToTwoCredits = subjects.Count(c => c >= 1 && c <= 2),
                ThreeToFourCredits = subjects.Count(c => c >= 3 && c <= 4),
                FivePlusCredits = subjects.Count(c => c >= 5)
            };
        }

        public async Task<SyllabusAvailability> GetSyllabusAvailabilityAsync()
        {
            var subjectsWithSyllabus = await _context.Subjects
                .Where(s => s.ApprovalStatus == EApprovalStatus.APPROVED && !s.IsDeleted)
                .Where(s => s.SubjectVersions.Any(sv => 
                    !sv.IsDeleted && sv.Syllabi.Any(sy => !sy.IsDeleted)))
                .CountAsync();

            var totalSubjects = await _context.Subjects
                .CountAsync(s => s.ApprovalStatus == EApprovalStatus.APPROVED && !s.IsDeleted);

            var subjectsWithoutSyllabus = totalSubjects - subjectsWithSyllabus;
            var percentage = totalSubjects > 0 ? (double)subjectsWithSyllabus / totalSubjects * 100 : 0;

            return new SyllabusAvailability
            {
                SubjectsWithSyllabus = subjectsWithSyllabus,
                SubjectsWithoutSyllabus = subjectsWithoutSyllabus,
                PercentageWithSyllabus = Math.Round(percentage, 2)
            };
        }

        public async Task<List<SubjectVersionStats>> GetTopSubjectsWithMostVersionsAsync(int limit = 10)
        {
            return await _context.Subjects
                .Where(s => s.ApprovalStatus == EApprovalStatus.APPROVED && !s.IsDeleted)
                .Select(s => new SubjectVersionStats
                {
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    VersionCount = s.SubjectVersions.Count(sv => !sv.IsDeleted)
                })
                .Where(s => s.VersionCount > 0)
                .OrderByDescending(s => s.VersionCount)
                .Take(limit)
                .ToListAsync();
        }

        #endregion

        #region Curricula Statistics

        public async Task<List<CurriculaByProgramStats>> GetCurriculaByProgramAsync()
        {
            return await _context.Curricula
                .Where(c => c.ApprovalStatus == EApprovalStatus.APPROVED && !c.IsDeleted)
                .Include(c => c.Program)
                .GroupBy(c => new { c.Program.ProgramCode, c.Program.ProgramName })
                .Select(g => new CurriculaByProgramStats
                {
                    ProgramCode = g.Key.ProgramCode,
                    ProgramName = g.Key.ProgramName,
                    CurriculumCount = g.Count()
                })
                .OrderByDescending(c => c.CurriculumCount)
                .ToListAsync();
        }

        public async Task<AverageSubjectsPerCurriculum> GetAverageSubjectsPerCurriculumAsync()
        {
            var curriculumSubjectCounts = await _context.Curricula
                .Where(c => c.ApprovalStatus == EApprovalStatus.APPROVED && !c.IsDeleted)
                .Select(c => c.CurriculumSubjects.Count(cs => !cs.IsDeleted))
                .ToListAsync();

            if (!curriculumSubjectCounts.Any())
            {
                return new AverageSubjectsPerCurriculum();
            }

            return new AverageSubjectsPerCurriculum
            {
                Average = Math.Round(curriculumSubjectCounts.Average(), 2),
                MinSubjects = curriculumSubjectCounts.Min(),
                MaxSubjects = curriculumSubjectCounts.Max()
            };
        }

        public async Task<CurriculumSizeDistribution> GetCurriculumSizeDistributionAsync()
        {
            var curriculumSubjectCounts = await _context.Curricula
                .Where(c => c.ApprovalStatus == EApprovalStatus.APPROVED && !c.IsDeleted)
                .Select(c => c.CurriculumSubjects.Count(cs => !cs.IsDeleted))
                .ToListAsync();

            return new CurriculumSizeDistribution
            {
                LessThan30Subjects = curriculumSubjectCounts.Count(c => c < 30),
                Between30And50Subjects = curriculumSubjectCounts.Count(c => c >= 30 && c <= 50),
                MoreThan50Subjects = curriculumSubjectCounts.Count(c => c > 50)
            };
        }

        public async Task<SemesterCompleteness> GetSemesterCompletenessAsync()
        {
            var curriculaWithEightSemesters = await _context.Curricula
                .Where(c => c.ApprovalStatus == EApprovalStatus.APPROVED && !c.IsDeleted)
                .Where(c => c.CurriculumSubjects
                    .Where(cs => !cs.IsDeleted)
                    .Select(cs => cs.SemesterNumber)
                    .Distinct()
                    .Count() >= 8)
                .CountAsync();

            var totalCurricula = await _context.Curricula
                .CountAsync(c => c.ApprovalStatus == EApprovalStatus.APPROVED && !c.IsDeleted);

            var percentage = totalCurricula > 0 ? (double)curriculaWithEightSemesters / totalCurricula * 100 : 0;

            return new SemesterCompleteness
            {
                CurriculaWithFullEightSemesters = curriculaWithEightSemesters,
                TotalCurricula = totalCurricula,
                PercentageComplete = Math.Round(percentage, 2)
            };
        }

        #endregion

        #region Recent Activities

        public async Task<List<RecentSubject>> GetRecentSubjectsAsync(int days = 7, int limit = 10)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            return await _context.Subjects
                .Where(s => !s.IsDeleted && s.CreatedAt >= cutoffDate)
                .OrderByDescending(s => s.CreatedAt)
                .Take(limit)
                .Select(s => new RecentSubject
                {
                    Id = s.Id,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credits = s.Credits,
                    CreatedBy = s.CreatedBy,
                    CreatedAt = s.CreatedAt ?? DateTime.MinValue
                })
                .ToListAsync();
        }

        public async Task<List<RecentSyllabus>> GetRecentlyApprovedSyllabiAsync(int days = 7, int limit = 10)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            return await _context.Syllabi
                .Where(s => !s.IsDeleted
                    && s.ApprovalStatus == EApprovalStatus.APPROVED
                    && s.ApprovedAt >= cutoffDate)
                .Include(s => s.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .OrderByDescending(s => s.ApprovedAt)
                .Take(limit)
                .Select(s => new RecentSyllabus
                {
                    Id = s.Id,
                    SubjectCode = s.SubjectVersion.Subject.SubjectCode,
                    SubjectName = s.SubjectVersion.Subject.SubjectName,
                    VersionCode = s.SubjectVersion.VersionCode,
                    ApprovedBy = s.ApprovedBy,
                    ApprovedAt = s.ApprovedAt ?? DateTime.MinValue
                })
                .ToListAsync();
        }

        public async Task<List<PendingSubject>> GetPendingSubjectsAsync(int limit = 20)
        {
            var now = DateTime.UtcNow;

            return await _context.Subjects
                .Where(s => !s.IsDeleted && s.ApprovalStatus == EApprovalStatus.PENDING)
                .OrderBy(s => s.CreatedAt)
                .Take(limit)
                .Select(s => new PendingSubject
                {
                    Id = s.Id,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credits = s.Credits,
                    CreatedBy = s.CreatedBy,
                    CreatedAt = s.CreatedAt ?? DateTime.MinValue,
                    DaysPending = (int)(now - (s.CreatedAt ?? DateTime.MinValue)).TotalDays
                })
                .ToListAsync();
        }

        public async Task<List<ExpiringSubjectVersion>> GetExpiringSoonSubjectVersionsAsync(int days = 30, int limit = 15)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(days);
            var now = DateTime.UtcNow;

            return await _context.SubjectVersions
                .Where(sv => !sv.IsDeleted
                    && sv.IsActive
                    && sv.EffectiveTo != null
                    && sv.EffectiveTo >= now
                    && sv.EffectiveTo <= cutoffDate)
                .Include(sv => sv.Subject)
                .OrderBy(sv => sv.EffectiveTo)
                .Take(limit)
                .Select(sv => new ExpiringSubjectVersion
                {
                    Id = sv.Id,
                    SubjectCode = sv.Subject.SubjectCode,
                    SubjectName = sv.Subject.SubjectName,
                    VersionCode = sv.VersionCode,
                    EffectiveTo = sv.EffectiveTo ?? DateTime.MinValue,
                    DaysUntilExpiry = (int)(sv.EffectiveTo!.Value - now).TotalDays
                })
                .ToListAsync();
        }

        #endregion
    }
}
