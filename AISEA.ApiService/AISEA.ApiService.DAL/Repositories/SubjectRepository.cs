using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Responses.Subject;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>
    {
        public SubjectRepository(AiseaContext context) : base(context)
        {
        }
        public async Task<List<SimpleSubjectResponse>> GetAllViaCurriculumNotIncludeComboAsync(string studentCurriculumCode)
        {


            var subjects = await _context.CurriculumSubjects
                .Where(cs =>
                    cs.Curriculum != null &&
                    cs.Curriculum.CurriculumCode == studentCurriculumCode &&

                    cs.SubjectVersion != null &&
                    cs.SubjectVersion.Subject != null &&

                    !cs.IsDeleted &&
                    !cs.SubjectVersion.IsDeleted &&
                    !cs.SubjectVersion.Subject.IsDeleted &&
                    cs.SubjectVersion.Subject.ApprovalStatus == EApprovalStatus.APPROVED &&

                    !_context.ComboSubjects.Any(cb =>
                        cb.SubjectId == cs.SubjectVersion.Subject.Id &&
                        !cb.IsDeleted))
                .Select(cs => new SimpleSubjectResponse
                {
                    Id = cs.SubjectVersion.Subject.Id,
                    SubjectCode = cs.SubjectVersion.Subject.SubjectCode,
                    SubjectName = cs.SubjectVersion.Subject.SubjectName,
                    Credits = cs.SubjectVersion.Subject.Credits,
                    SemesterNumber = cs.SemesterNumber
                })
                .ToListAsync();

            return subjects;
        }


        public async Task<List<SimpleSubjectResponse>> GetAllViaComboNameAsync(string studentComboName)
        {
            var subjects = await _context.ComboSubjects
                .Where(cs =>
                    cs.Combo != null &&
                    cs.Combo.ComboName == studentComboName &&

                    !cs.IsDeleted &&

                    cs.Combo.IsDeleted == false &&
                    cs.Combo.ApprovalStatus == EApprovalStatus.APPROVED &&


                    cs.Subject != null &&
                    !cs.Subject.IsDeleted &&
                    cs.Subject.ApprovalStatus == EApprovalStatus.APPROVED &&


                    cs.Subject.SubjectVersions.Any(sv =>
                        !sv.IsDeleted
                        && sv.IsActive
                        && _context.CurriculumSubjects.Any(currSub =>
                            currSub.SubjectVersionId == sv.Id &&
                            !currSub.IsDeleted)))


                .Select(cs => new SimpleSubjectResponse
                {
                    Id = cs.Subject.Id,
                    SubjectCode = cs.Subject.SubjectCode,
                    SubjectName = cs.Subject.SubjectName,
                    Credits = cs.Subject.Credits,
                    SemesterNumber = _context.CurriculumSubjects
                        .Where(currSub => currSub.SubjectVersion.SubjectId == cs.Subject.Id && !currSub.IsDeleted)
                        .Select(currSub => currSub.SemesterNumber)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return subjects;
        }


        public async Task<Subject?> GetByCodeAsync(string subjectCode)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode && !s.IsDeleted);
        }

        public async Task<(IEnumerable<Subject> Subjects, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search = null, string? comboName = null, string? curriculumCode = null)
        {
            var query = _context.Subjects.Where(s => !s.IsDeleted);

            // Search by SubjectCode or SubjectName
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.SubjectName.Contains(search) || s.SubjectCode.Contains(search));
            }

            // Filter by Combo Name
            if (!string.IsNullOrEmpty(comboName))
            {
                query = query.Where(s => s.ComboSubjects.Any(cs => cs.Combo.ComboName.Contains(comboName) && !cs.Combo.IsDeleted));
            }

            // Filter by CurriculumCode
            if (!string.IsNullOrEmpty(curriculumCode))
            {
                query = query.Where(s => s.SubjectVersions.Any(sv =>
                    sv.CurriculumSubjects.Any(cs => cs.Curriculum.CurriculumCode.Contains(curriculumCode) && !cs.Curriculum.IsDeleted)));
            }

            var totalCount = await query.CountAsync();
            var subjects = await query
                .OrderBy(s => s.SubjectCode)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (subjects, totalCount);
        }

        public async Task<List<Subject>> GetByIdsAsync(List<long> subjectIds)
        {
            return await _context.Subjects
                .Where(s => subjectIds.Contains(s.Id) && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<CheckToDeactiveSubjectDTO>> GetAllViaCurriculumAsync(string curriculumCode)
        {
            var result = await _context.Curricula
                .Where(c => c.CurriculumCode == curriculumCode)
                .SelectMany(c => c.CurriculumSubjects)
                .Select(cs => cs.SubjectVersion.Subject)
                .Select(s => new CheckToDeactiveSubjectDTO
                {
                    SubjectId = s.Id,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Combos = s.ComboSubjects
                        .Select(cs => cs.Combo.ComboName)
                        .ToList()
                })
                .ToListAsync();

            return result;
        }


        public async Task<ImportableSubjectDTO> GetSubjectWCurNComNPreNVerAsync(string subjectCode)
        {
            var now = DateTime.Now;
            return await _context.Subjects
                .Where(s => s.SubjectCode == subjectCode
                    && !s.IsDeleted
                    && s.ApprovalStatus == SHARED.Const.Enums.EApprovalStatus.APPROVED)
                .Select(s => new ImportableSubjectDTO
                {
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credits = s.Credits,
                    Description = s.Description,


                    // Prerequisite subject codes (filtered)

                    PrerequisiteSubjectCodes = s.SubjectVersions
                .Where(v => !v.IsDeleted
                    && v.IsActive
                    && v.EffectiveFrom <= now
                    && (v.EffectiveTo == null || now <= v.EffectiveTo))
                .SelectMany(v => v.Prerequisites)
                .Where(p => !p.PrerequisiteSubjectVersion.IsDeleted
                    && p.PrerequisiteSubjectVersion.IsActive
                    && p.PrerequisiteSubjectVersion.EffectiveFrom <= now
                    && (p.PrerequisiteSubjectVersion.EffectiveTo == null || now <= p.PrerequisiteSubjectVersion.EffectiveTo)
                    && !p.PrerequisiteSubjectVersion.Subject.IsDeleted
                    && p.PrerequisiteSubjectVersion.Subject.ApprovalStatus == SHARED.Const.Enums.EApprovalStatus.APPROVED)
                .Select(p => p.PrerequisiteSubjectVersion.Subject.SubjectCode)
                .Distinct()
                .ToList(),

                    // Versions (filtered)
                    Versions = s.SubjectVersions
                        .Where(v => !v.IsDeleted
                            && v.IsActive
                            && v.EffectiveFrom <= DateTime.Now
                            && (v.EffectiveTo == null || DateTime.Now <= v.EffectiveTo))
                        .Select(v => v.VersionCode)
                        .Distinct()
                        .ToList(),

                    // Curricula (filtered)
                    CurriculumCodes = s.SubjectVersions
                        .Where(v => !v.IsDeleted
                            && v.IsActive
                            && v.EffectiveFrom <= DateTime.Now
                            && (v.EffectiveTo == null || DateTime.Now <= v.EffectiveTo))
                        .SelectMany(v => v.CurriculumSubjects)
                        .Where(cs => !cs.Curriculum.IsDeleted
                            && cs.Curriculum.ApprovalStatus == SHARED.Const.Enums.EApprovalStatus.APPROVED)
                        .Select(cs => cs.Curriculum.CurriculumCode)
                        .Distinct()
                        .ToList(),

                    // Combos (filtered)
                    ComboNames = s.ComboSubjects
                        .Where(cs => !cs.Combo.IsDeleted
                            && cs.Combo.ApprovalStatus == SHARED.Const.Enums.EApprovalStatus.APPROVED)
                        .Select(cs => cs.Combo.ComboName)
                        .Distinct()
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

    }
}