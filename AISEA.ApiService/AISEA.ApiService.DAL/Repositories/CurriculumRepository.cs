using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class CurriculumRepository : GenericRepository<Curriculum>
    {
        public CurriculumRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<Curriculum?> GetByCodeAsync(string curriculumCode)
        {
            return await _context.Curricula
                .Include(c => c.Program)
                .FirstOrDefaultAsync(c => c.CurriculumCode == curriculumCode && !c.IsDeleted);
        }

        public async Task<Curriculum?> GetDetailByIdAsync(long id)
        {
            return await _context.Curricula
                .Include(c => c.Program)
                .Include(c => c.CurriculumSubjects.Where(cs => !cs.IsDeleted))
                    .ThenInclude(cs => cs.SubjectVersion)
                        .ThenInclude(sv => sv.Subject)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<(IEnumerable<Curriculum> Curricula, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search = null, long? programId = null)
        {
            var query = _context.Curricula
                .Include(c => c.Program)
                .Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CurriculumName.Contains(search) || c.CurriculumCode.Contains(search));
            }

            if (programId.HasValue)
            {
                query = query.Where(c => c.ProgramId == programId.Value);
            }

            var totalCount = await query.CountAsync();
            var curricula = await query
                .OrderBy(c => c.CurriculumCode)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (curricula, totalCount);
        }

        public async Task<bool> IsCodeUniqueAsync(string curriculumCode, long? excludeId = null)
        {
            var query = _context.Curricula.Where(c => c.CurriculumCode == curriculumCode && !c.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> HasSubjectsAsync(long curriculumId)
        {
            return await _context.CurriculumSubjects
                .AnyAsync(cs => cs.CurriculumId == curriculumId && !cs.IsDeleted);
        }

        public async Task<AcademicDataDto> GetAllAcademicDataAsync()
        {
            // Fetch Programs
            var programs = await _context.Programs
                .Where(p => !p.IsDeleted)
                .Select(p => new ProgramDto
                {
                    Id = p.Id,
                    ProgramName = p.ProgramName,
                    ProgramCode = p.ProgramCode,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            // Fetch Curricula with related data
            var curricula = await _context.Curricula
                .Where(c => !c.IsDeleted && c.ApprovalStatus == EApprovalStatus.APPROVED)
                .Include(c => c.Program)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                    .ThenInclude(s => s.ComboSubjects)
                    .ThenInclude(cbs => cbs.Combo)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Syllabi)
                    .ThenInclude(s => s.SyllabusAssessments)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Syllabi)
                    .ThenInclude(s => s.SyllabusLearningMaterials)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Syllabi)
                    .ThenInclude(s => s.SyllabusLearningOutcomes)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Syllabi)
                    .ThenInclude(s => s.SyllabusSessions)
                    .ThenInclude(ss => ss.SessionOutcomeMappings)
                    .ThenInclude(som => som.Outcome)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Prerequisites)
                    .ThenInclude(pr => pr.PrerequisiteSubjectVersion)
                    .ThenInclude(psv => psv.Subject)
                .Where(c => c.CurriculumSubjects.Any(cs => !cs.IsDeleted && cs.SubjectVersion.IsActive && !cs.SubjectVersion.IsDeleted))
                .Select(c => new CurriculumDto
                {
                    Id = c.Id,
                    ProgramId = c.ProgramId,
                    ProgramName = c.Program.ProgramName,
                    CurriculumCode = c.CurriculumCode,
                    CurriculumName = c.CurriculumName,
                    EffectiveDate = c.EffectiveDate,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ApprovedAt = c.ApprovedAt,
                    ApprovedBy = c.ApprovedBy,
                    CurriculumSubjects = c.CurriculumSubjects
                        .Where(cs => !cs.IsDeleted)
                        .Select(cs => new CurriculumSubjectDto
                        {
                            SemesterNumber = cs.SemesterNumber,
                            IsMandatory = cs.IsMandatory,
                            SubjectVersion = new SubjectVersionDto
                            {
                                Id = cs.SubjectVersion.Id,
                                VersionCode = cs.SubjectVersion.VersionCode,
                                VersionName = cs.SubjectVersion.VersionName,
                                IsActive = cs.SubjectVersion.IsActive,
                                IsDefault = cs.SubjectVersion.IsDefault,
                                EffectiveFrom = cs.SubjectVersion.EffectiveFrom,
                                EffectiveTo = cs.SubjectVersion.EffectiveTo,
                                Subject = new SubjectDto
                                {
                                    Id = cs.SubjectVersion.Subject.Id,
                                    SubjectCode = cs.SubjectVersion.Subject.SubjectCode,
                                    SubjectName = cs.SubjectVersion.Subject.SubjectName,
                                    Credits = cs.SubjectVersion.Subject.Credits,
                                    Description = cs.SubjectVersion.Subject.Description,
                                    Combos = cs.SubjectVersion.Subject.ComboSubjects
                                        .Where(cbs => !cbs.IsDeleted)
                                        .Select(cbs => new ComboDto
                                        {
                                            Id = cbs.Combo.Id,
                                            ComboName = cbs.Combo.ComboName,
                                            ComboDescription = cbs.Combo.ComboDescription
                                        }).ToList()
                                },
                                Syllabi = cs.SubjectVersion.Syllabi
                                    .Where(s => !s.IsDeleted && s.ApprovalStatus == EApprovalStatus.APPROVED)
                                    .Select(s => new SyllabusDto
                                    {
                                        Id = s.Id,
                                        Content = s.Content,
                                        Assessments = s.SyllabusAssessments
                                            .Where(sa => !sa.IsDeleted)
                                            .Select(sa => new SyllabusAssessmentDto
                                            {
                                                Id = sa.Id,
                                                Category = sa.Category,
                                                Quantity = sa.Quantity,
                                                Weight = sa.Weight,
                                                CompletionCriteria = sa.CompletionCriteria,
                                                Duration = sa.Duration,
                                                QuestionType = sa.QuestionType
                                            }).ToList(),
                                        LearningMaterials = s.SyllabusLearningMaterials
                                            .Where(lm => !lm.IsDeleted)
                                            .Select(lm => new SyllabusLearningMaterialDto
                                            {
                                                Id = lm.Id,
                                                MaterialName = lm.MaterialName,
                                                AuthorName = lm.AuthorName,
                                                PublishedDate = lm.PublishedDate,
                                                Description = lm.Description,
                                                FilepathOrUrl = lm.FilepathOrUrl
                                            }).ToList(),
                                        LearningOutcomes = s.SyllabusLearningOutcomes
                                            .Where(lo => !lo.IsDeleted)
                                            .Select(lo => new SyllabusLearningOutcomeDto
                                            {
                                                Id = lo.Id,
                                                OutcomeCode = lo.OutcomeCode,
                                                Description = lo.Description
                                            }).ToList(),
                                        Sessions = s.SyllabusSessions
                                            .Where(ss => !ss.IsDeleted)
                                            .Select(ss => new SyllabusSessionDto
                                            {
                                                Id = ss.Id,
                                                SessionNumber = ss.SessionNumber,
                                                Topic = ss.Topic,
                                                Mission = ss.Mission,
                                                Outcomes = ss.SessionOutcomeMappings
                                                    .Where(som => !som.IsDeleted)
                                                    .Select(som => new SyllabusLearningOutcomeDto
                                                    {
                                                        Id = som.Outcome.Id,
                                                        OutcomeCode = som.Outcome.OutcomeCode,
                                                        Description = som.Outcome.Description
                                                    }).ToList()
                                            }).ToList()
                                    }).ToList(),
                                Prerequisites = cs.SubjectVersion.Prerequisites
                                    .Where(pr => !pr.IsDeleted)
                                    .Select(pr => new SubjectVersionPrerequisiteDto
                                    {
                                        PrerequisiteSubjectVersionId = pr.PrerequisiteSubjectVersionId,
                                        PrerequisiteSubjectCode = pr.PrerequisiteSubjectVersion.Subject.SubjectCode,
                                        PrerequisiteSubjectName = pr.PrerequisiteSubjectVersion.Subject.SubjectName
                                    }).ToList()
                            }
                        }).ToList()
                })
                .ToListAsync();

            // Fetch Semesters
            var semesters = await _context.Semesters
                .Select(s => new SemesterDto
                {
                    Id = s.Id,
                    SemesterName = s.SemesterName,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();



            return new AcademicDataDto
            {
                Programs = programs,
                Curricula = curricula,
                Semesters = semesters,
            };
        }
    }
}


#region DTO

public class AcademicDataDto
{
    public List<ProgramDto> Programs { get; set; }
    public List<CurriculumDto> Curricula { get; set; }
    public List<SemesterDto> Semesters { get; set; }
}

public class ProgramDto
{
    public long Id { get; set; }
    public string ProgramName { get; set; }
    public string ProgramCode { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CurriculumDto
{
    public long Id { get; set; }
    public long ProgramId { get; set; }
    public string ProgramName { get; set; }
    public string CurriculumCode { get; set; }
    public string CurriculumName { get; set; }
    public DateTimeOffset EffectiveDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string ApprovedBy { get; set; }
    public List<CurriculumSubjectDto> CurriculumSubjects { get; set; }
}

public class CurriculumSubjectDto
{
    public int SemesterNumber { get; set; }
    public bool IsMandatory { get; set; }
    public SubjectVersionDto SubjectVersion { get; set; }
}

public class SubjectVersionDto
{
    public long Id { get; set; }
    public string VersionCode { get; set; }
    public string VersionName { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public SubjectDto Subject { get; set; }
    public List<SyllabusDto> Syllabi { get; set; }
    public List<SubjectVersionPrerequisiteDto> Prerequisites { get; set; }
}

public class SubjectDto
{
    public long Id { get; set; }
    public string SubjectCode { get; set; }
    public string SubjectName { get; set; }
    public int Credits { get; set; }
    public string Description { get; set; }
    public List<ComboDto> Combos { get; set; }
}

public class ComboDto
{
    public long Id { get; set; }
    public string ComboName { get; set; }
    public string ComboDescription { get; set; }
}

public class SyllabusDto
{
    public long Id { get; set; }
    public string Content { get; set; }
    public List<SyllabusAssessmentDto> Assessments { get; set; }
    public List<SyllabusLearningMaterialDto> LearningMaterials { get; set; }
    public List<SyllabusLearningOutcomeDto> LearningOutcomes { get; set; }
    public List<SyllabusSessionDto> Sessions { get; set; }
}

public class SyllabusAssessmentDto
{
    public long Id { get; set; }
    public string Category { get; set; }
    public int Quantity { get; set; }
    public decimal Weight { get; set; }
    public string CompletionCriteria { get; set; }
    public int? Duration { get; set; }
    public string QuestionType { get; set; }
}

public class SyllabusLearningMaterialDto
{
    public long Id { get; set; }
    public string MaterialName { get; set; }
    public string AuthorName { get; set; }
    public DateTimeOffset? PublishedDate { get; set; }
    public string Description { get; set; }
    public string FilepathOrUrl { get; set; }
}

public class SyllabusLearningOutcomeDto
{
    public long Id { get; set; }
    public string OutcomeCode { get; set; }
    public string Description { get; set; }
}

public class SyllabusSessionDto
{
    public long Id { get; set; }
    public int SessionNumber { get; set; }
    public string Topic { get; set; }
    public string Mission { get; set; }
    public List<SyllabusLearningOutcomeDto> Outcomes { get; set; }
}

public class SubjectVersionPrerequisiteDto
{
    public long PrerequisiteSubjectVersionId { get; set; }
    public string PrerequisiteSubjectCode { get; set; }
    public string PrerequisiteSubjectName { get; set; }
}

public class SemesterDto
{
    public long Id { get; set; }
    public string SemesterName { get; set; }
    public DateTime CreatedAt { get; set; }
}

#endregion