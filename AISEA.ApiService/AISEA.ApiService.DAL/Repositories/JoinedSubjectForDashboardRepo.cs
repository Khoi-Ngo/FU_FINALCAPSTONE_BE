using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    /// <summary>
    /// Repository with read-only, aggregation-heavy queries powering
    /// student self dashboards and admin observation views.
    ///
    /// NOTE: All DTOs used here are defined below in the same file
    /// per the request. Only the provided schema is used.
    /// </summary>
    public class JoinedSubjectForDashboardRepo : GenericRepository<JoinedSubject>
    {
        public JoinedSubjectForDashboardRepo(AiseaContext context) : base(context)
        {
        }

        #region ===== Student-facing analytics (4 advanced graphs) =====
        /// <summary>
        /// 1) Semester performance trend per student.
        /// - subjects attempted & passed
        /// - credits attempted & earned
        /// - average subject final score (weighted by assessment Weight within each subject)
        /// </summary>
        public async Task<IReadOnlyList<StudentSemesterPerformanceDto>> GetStudentSemesterPerformanceAsync(
            long studentProfileId,
            CancellationToken ct = default)
        {
            // Preload weighted final scores per JoinedSubject for this student
            var subjectFinals = await _context.JoinedSubjects
                .AsNoTracking()
                .Where(js => js.StudentProfileId == studentProfileId)
                .Select(js => new
                {
                    js.Id,
                    js.SemesterId,
                    js.Credits,
                    js.IsPassed,
                    FinalScore = js.SubjectMarkReports
                        .Select(m => new { m.Weight, m.Score })
                        .DefaultIfEmpty()
                        .Select(x => x)
                        .ToList()
                })
                .ToListAsync(ct);

            // Compute final score per subject = sum(weight*score) / sum(weight)
            var finalsBySubject = subjectFinals.Select(x => new
            {
                x.Id,
                x.SemesterId,
                x.Credits,
                x.IsPassed,
                FinalScore = SafeWeightedAverage(x.FinalScore)
            });

            // Group by semester
            var semesterIds = finalsBySubject.Select(s => s.SemesterId).Distinct().ToList();
            var semesterLookup = await _context.Semesters
                .AsNoTracking()
                .Where(s => semesterIds.Contains(s.Id))
                .Select(s => new { s.Id, s.SemesterName })
                .ToDictionaryAsync(s => s.Id, s => s.SemesterName, ct);

            var result = finalsBySubject
                .GroupBy(s => s.SemesterId)
                .Select(g => new StudentSemesterPerformanceDto
                {
                    SemesterId = g.Key,
                    SemesterName = semesterLookup.TryGetValue(g.Key, out var name) ? name : $"Semester #{g.Key}",
                    SubjectsAttempted = g.Count(),
                    SubjectsPassed = g.Count(x => x.IsPassed),
                    CreditsAttempted = g.Sum(x => x.Credits ?? 0),
                    CreditsEarned = g.Where(x => x.IsPassed).Sum(x => x.Credits ?? 0),
                    AverageFinalScore = SafeAverage(g.Select(x => x.FinalScore))
                })
                .OrderBy(x => x.SemesterName)
                .ToList();

            return result;
        }

        /// <summary>
        /// 2) Assessment category performance breakdown (per student across all subjects).
        /// Returns average score per Category and the total Weight considered.
        /// Intended for donut/bar chart by Category.
        /// </summary>
        public async Task<IReadOnlyList<StudentCategoryPerformanceDto>> GetStudentCategoryPerformanceAsync(
            long studentProfileId,
            CancellationToken ct = default)
        {
            var q = from js in _context.JoinedSubjects.AsNoTracking()
                    where js.StudentProfileId == studentProfileId
                    from m in js.SubjectMarkReports
                    group m by m.Category into g
                    select new StudentCategoryPerformanceDto
                    {
                        Category = g.Key,
                        AverageScore = g.Average(x => x.Score),
                        TotalWeight = g.Sum(x => x.Weight)
                    };

            return await q.OrderByDescending(x => x.TotalWeight)
                           .ThenBy(x => x.Category)
                           .ToListAsync(ct);
        }

        /// <summary>
        /// 3) Checkpoint status timeline (student) bucketed by month.
        /// Shows Total, Completed, and Overdue counts by YearMonth derived from Deadline.
        /// Useful as a stacked column chart over time.
        /// </summary>
        public async Task<IReadOnlyList<StudentCheckpointTimelineDto>> GetStudentCheckpointTimelineAsync(
            long studentProfileId,
            DateTime? startInclusive = null,
            DateTime? endInclusive = null,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var baseQuery = _context.JoinedSubjectCheckPoints
                .AsNoTracking()
                .Where(cp => cp.JoinedSubject.StudentProfileId == studentProfileId);

            if (startInclusive.HasValue)
                baseQuery = baseQuery.Where(cp => cp.Deadline >= startInclusive.Value);
            if (endInclusive.HasValue)
                baseQuery = baseQuery.Where(cp => cp.Deadline <= endInclusive.Value);

            var q = from cp in baseQuery
                    let ym = new { Year = cp.Deadline.Year, Month = cp.Deadline.Month }
                    group cp by ym into g
                    orderby g.Key.Year, g.Key.Month
                    select new StudentCheckpointTimelineDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Total = g.Count(),
                        Completed = g.Count(x => x.IsCompleted),
                        Overdue = g.Count(x => !x.IsCompleted && x.Deadline < now)
                    };

            return await q.ToListAsync(ct);
        }

        /// <summary>
        /// 4) Final-score distribution (student) into configurable buckets.
        /// Great for histogram. Buckets default to: 0-50, 50-65, 65-80, 80-90, 90-100.
        /// </summary>
        public async Task<IReadOnlyList<StudentScoreBucketDto>> GetStudentScoreDistributionAsync(
            long studentProfileId,
            IReadOnlyList<ScoreBucket> buckets = null,
            CancellationToken ct = default)
        {
            var defaultBuckets = new List<ScoreBucket>
            {
                new(0,50), new(50,65), new(65,80), new(80,90), new(90,100)
            };
            buckets ??= defaultBuckets;

            // Pull subject-level final score for the student
            var raw = await _context.JoinedSubjects
                .AsNoTracking()
                .Where(js => js.StudentProfileId == studentProfileId)
                .Select(js => new
                {
                    js.Id,
                    FinalScore = js.SubjectMarkReports
                        .Select(m => new { m.Weight, m.Score })
                        .ToList()
                })
                .ToListAsync(ct);

            var finals = raw.Select(x => SafeWeightedAverage(x.FinalScore))
                            .Where(s => s.HasValue)
                            .Select(s => s!.Value)
                            .ToList();

            var dist = buckets.Select(b => new StudentScoreBucketDto
            {
                BucketLabel = b.ToString(),
                Count = finals.Count(v => v >= b.Min && v < b.Max)
            }).ToList();

            return dist;
        }
        #endregion

        #region ===== Admin observation (tables/graphs) =====
        /// <summary>
        /// Pass rate by semester across all students.
        /// </summary>
        public async Task<IReadOnlyList<AdminSemesterPassRateDto>> GetPassRateBySemesterAsync(
            CancellationToken ct = default)
        {
            var q = from js in _context.JoinedSubjects.AsNoTracking()
                    group js by js.SemesterId into g
                    select new
                    {
                        SemesterId = g.Key,
                        Attempted = g.Count(),
                        Passed = g.Count(x => x.IsPassed)
                    };

            var tmp = await q.ToListAsync(ct);
            var semesterNames = await _context.Semesters.AsNoTracking()
                .Where(s => tmp.Select(t => t.SemesterId).Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.SemesterName, ct);

            return tmp.Select(t => new AdminSemesterPassRateDto
            {
                SemesterId = t.SemesterId,
                SemesterName = semesterNames.TryGetValue(t.SemesterId, out var n) ? n : $"Semester #{t.SemesterId}",
                Attempted = t.Attempted,
                Passed = t.Passed,
                PassRate = t.Attempted == 0 ? 0 : (double)t.Passed / t.Attempted
            }).OrderBy(x => x.SemesterName).ToList();
        }

        /// <summary>
        /// Average final score by SubjectCode (across semesters/students).
        /// </summary>
        public async Task<IReadOnlyList<AdminAverageScoreBySubjectDto>> GetAverageScoreBySubjectAsync(
            CancellationToken ct = default)
        {
            var subjectFinals = await _context.JoinedSubjects
                .AsNoTracking()
                .Select(js => new
                {
                    js.SubjectCode,
                    js.SubjectVersionCode,
                    FinalScore = js.SubjectMarkReports.Select(m => new { m.Weight, m.Score }).ToList()
                })
                .ToListAsync(ct);

            var bySubject = subjectFinals
                .Select(x => new { x.SubjectCode, x.SubjectVersionCode, Final = SafeWeightedAverage(x.FinalScore) })
                .Where(x => x.Final.HasValue)
                .GroupBy(x => new { x.SubjectCode, x.SubjectVersionCode })
                .Select(g => new AdminAverageScoreBySubjectDto
                {
                    SubjectCode = g.Key.SubjectCode,
                    SubjectVersionCode = g.Key.SubjectVersionCode,
                    AverageFinalScore = g.Average(v => v.Final!.Value),
                    Attempts = g.Count()
                })
                .OrderByDescending(x => x.AverageFinalScore)
                .ThenBy(x => x.SubjectCode)
                .ToList();

            return bySubject;
        }

        /// <summary>
        /// Overdue checkpoints by semester (admin overview).
        /// </summary>
        public async Task<IReadOnlyList<AdminOverdueCheckpointBySemesterDto>> GetOverdueCheckpointsBySemesterAsync(
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var q = from cp in _context.JoinedSubjectCheckPoints.AsNoTracking()
                    let js = cp.JoinedSubject
                    group cp by js.SemesterId into g
                    select new
                    {
                        SemesterId = g.Key,
                        Overdue = g.Count(x => !x.IsCompleted && x.Deadline < now),
                        Total = g.Count()
                    };

            var tmp = await q.ToListAsync(ct);
            var semesterNames = await _context.Semesters.AsNoTracking()
                .Where(s => tmp.Select(t => t.SemesterId).Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.SemesterName, ct);

            return tmp.Select(t => new AdminOverdueCheckpointBySemesterDto
            {
                SemesterId = t.SemesterId,
                SemesterName = semesterNames.TryGetValue(t.SemesterId, out var n) ? n : $"Semester #{t.SemesterId}",
                TotalCheckpoints = t.Total,
                OverdueCheckpoints = t.Overdue,
                OverdueRate = t.Total == 0 ? 0 : (double)t.Overdue / t.Total
            }).OrderBy(x => x.SemesterName).ToList();
        }

        /// <summary>
        /// Student risk summary based on low final score and overdue checkpoints.
        /// Thresholds are adjustable. Uses only available schema.
        /// </summary>
        public async Task<IReadOnlyList<AdminStudentRiskDto>> GetStudentRiskSummaryAsync(
            double lowScoreThreshold = 60,
            int minOverdueCheckpoints = 2,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            // Final score per joined subject per student
            var subjectFinals = await _context.JoinedSubjects
                .AsNoTracking()
                .Select(js => new
                {
                    js.StudentProfileId,
                    js.Id,
                    FinalScore = js.SubjectMarkReports.Select(m => new { m.Weight, m.Score }).ToList()
                })
                .ToListAsync(ct);

            var byStudent = subjectFinals
                .GroupBy(x => x.StudentProfileId)
                .Select(g => new
                {
                    StudentProfileId = g.Key,
                    AverageFinalScore = SafeAverage(g.Select(v => SafeWeightedAverage(v.FinalScore)))
                })
                .ToList();

            // Overdue checkpoints per student
            var overdueByStudent = await _context.JoinedSubjectCheckPoints
                .AsNoTracking()
                .Where(cp => !cp.IsCompleted && cp.Deadline < now)
                .GroupBy(cp => cp.JoinedSubject.StudentProfileId)
                .Select(g => new { StudentProfileId = g.Key, Overdue = g.Count() })
                .ToDictionaryAsync(x => x.StudentProfileId, x => x.Overdue, ct);

            // Enrich with simple identity fields from StudentProfile
            var ids = byStudent.Select(x => x.StudentProfileId).Distinct().ToList();
            var studentLookup = await _context.StudentProfiles
                .AsNoTracking()
                .Where(s => ids.Contains(s.Id))
                .Select(s => new { s.Id, s.UserId, s.CurriculumCode, s.RegisteredComboCode })
                .ToDictionaryAsync(s => s.Id, s => s, ct);

            var result = byStudent.Select(x => new AdminStudentRiskDto
            {
                StudentProfileId = x.StudentProfileId,
                UserId = studentLookup.TryGetValue(x.StudentProfileId, out var u) ? u.UserId : 0,
                CurriculumCode = studentLookup.TryGetValue(x.StudentProfileId, out u) ? u.CurriculumCode : string.Empty,
                RegisteredComboCode = studentLookup.TryGetValue(x.StudentProfileId, out u) ? u.RegisteredComboCode : string.Empty,
                AverageFinalScore = x.AverageFinalScore,
                OverdueCheckpoints = overdueByStudent.TryGetValue(x.StudentProfileId, out var o) ? o : 0,
                IsLowScore = x.AverageFinalScore.HasValue && x.AverageFinalScore.Value < lowScoreThreshold,
                IsHighOverdue = overdueByStudent.TryGetValue(x.StudentProfileId, out var o2) && o2 >= minOverdueCheckpoints
            })
            .Where(r => (r.IsLowScore ?? false) || r.IsHighOverdue)
            .OrderBy(r => r.AverageFinalScore)
            .ThenByDescending(r => r.OverdueCheckpoints)
            .ToList();

            return result;
        }
        #endregion

        #region ===== Helpers =====
        private static double? SafeWeightedAverage(IEnumerable<dynamic> entries)
        {
            if (entries == null) return null;
            double wSum = 0, wsSum = 0;
            foreach (var e in entries)
            {
                if (e == null) continue;
                var w = Convert.ToDouble(e.Weight);
                var s = Convert.ToDouble(e.Score);
                wSum += w;
                wsSum += w * s;
            }
            if (wSum <= 0) return null;
            return wsSum / wSum;
        }

        private static double? SafeAverage(IEnumerable<double?> values)
        {
            var list = values?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
            if (list == null || list.Count == 0) return null;
            return list.Average();
        }
        #endregion
    }

    #region ===== DTOs (student) =====
    public class StudentSemesterPerformanceDto
    {
        public long SemesterId { get; set; }
        public string SemesterName { get; set; }
        public int SubjectsAttempted { get; set; }
        public int SubjectsPassed { get; set; }
        public int CreditsAttempted { get; set; }
        public int CreditsEarned { get; set; }
        public double? AverageFinalScore { get; set; }
    }

    public class StudentCategoryPerformanceDto
    {
        public string Category { get; set; }
        public double AverageScore { get; set; }
        public double TotalWeight { get; set; }
    }

    public class StudentCheckpointTimelineDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }
        public int Completed { get; set; }
        public int Overdue { get; set; }
        public string YearMonthLabel => $"{Year:D4}-{Month:D2}";
    }

    public class ScoreBucket
    {
        public double Min { get; }
        public double Max { get; }
        public ScoreBucket(double min, double max)
        {
            Min = min; Max = max;
        }
        public override string ToString() => $"[{Min}-{Max})";
    }

    public class StudentScoreBucketDto
    {
        public string BucketLabel { get; set; }
        public int Count { get; set; }
    }
    #endregion

    #region ===== DTOs (admin) =====
    public class AdminSemesterPassRateDto
    {
        public long SemesterId { get; set; }
        public string SemesterName { get; set; }
        public int Attempted { get; set; }
        public int Passed { get; set; }
        public double PassRate { get; set; }
    }

    public class AdminAverageScoreBySubjectDto
    {
        public string SubjectCode { get; set; }
        public string SubjectVersionCode { get; set; }
        public double AverageFinalScore { get; set; }
        public int Attempts { get; set; }
    }

    public class AdminOverdueCheckpointBySemesterDto
    {
        public long SemesterId { get; set; }
        public string SemesterName { get; set; }
        public int TotalCheckpoints { get; set; }
        public int OverdueCheckpoints { get; set; }
        public double OverdueRate { get; set; }
    }

    public class AdminStudentRiskDto
    {
        public long StudentProfileId { get; set; }
        public long UserId { get; set; }
        public string CurriculumCode { get; set; }
        public string RegisteredComboCode { get; set; }
        public double? AverageFinalScore { get; set; }
        public int OverdueCheckpoints { get; set; }
        public bool? IsLowScore { get; set; }
        public bool IsHighOverdue { get; set; }
    }
    #endregion
}
