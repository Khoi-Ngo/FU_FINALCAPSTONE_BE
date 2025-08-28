using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Responses.Subject;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>
    {
        public SubjectRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<Subject?> GetByCodeAsync(string subjectCode)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode && !s.IsDeleted);
        }

        public async Task<(IEnumerable<Subject> Subjects, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
        {
            var query = _context.Subjects.Where(s => !s.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.SubjectName.Contains(search) || s.SubjectCode.Contains(search));
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
        public async Task<Subject> GetApprovedNotDeleteByCodeAsync(string subjectCode)
        {
            return await _context.Subjects
            .Include(s => s.SubjectVersions).ThenInclude(sv => sv.CurriculumSubjects).ThenInclude(csv => csv.Curriculum)
            .Include(s => s.ComboSubjects).ThenInclude(cb => cb.Combo)
                .FirstOrDefaultAsync(s =>
                 s.SubjectCode == subjectCode
                && !s.IsDeleted
                && s.ApprovalStatus == SHARED.Const.Enums.EApprovalStatus.APPROVED);
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

    }
}