using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class CurriculumSubjectRepository : GenericRepository<CurriculumSubject>
    {
        public CurriculumSubjectRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(long curriculumId, long subjectVersionId)
        {
            return await _context.CurriculumSubjects
                .AnyAsync(cs => cs.CurriculumId == curriculumId && cs.SubjectVersionId == subjectVersionId && !cs.IsDeleted);
        }

        public async Task<bool> HasSubjectWithSubjectCodeAsync(long curriculumId, string subjectCode)
        {
            return await _context.CurriculumSubjects
                .Include(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Where(cs => cs.CurriculumId == curriculumId && !cs.IsDeleted)
                .AnyAsync(cs => cs.SubjectVersion.Subject.SubjectCode == subjectCode);
        }

        public async Task<List<CurriculumSubject>> GetByCurriculumIdAsync(long curriculumId)
        {
            return await _context.CurriculumSubjects
                .Include(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Where(cs => cs.CurriculumId == curriculumId && !cs.IsDeleted)
                .OrderBy(cs => cs.SemesterNumber)
                .ThenBy(cs => cs.SubjectVersion.Subject.SubjectCode)
                .ToListAsync();
        }

        public async Task RemoveSubjectFromCurriculumAsync(long curriculumId, long subjectVersionId)
        {
            var curriculumSubject = await _context.CurriculumSubjects
                .FirstOrDefaultAsync(cs => cs.CurriculumId == curriculumId && cs.SubjectVersionId == subjectVersionId && !cs.IsDeleted);
            
            if (curriculumSubject != null)
            {
                curriculumSubject.IsDeleted = true;
                curriculumSubject.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CurriculumSubject>> GetByCurriculumCodeAsync(string curriculumCode)
        {
            return await _context.CurriculumSubjects
                .Include(cs => cs.Curriculum)
                .Include(cs => cs.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Where(cs => cs.Curriculum.CurriculumCode == curriculumCode && !cs.IsDeleted && !cs.Curriculum.IsDeleted)
                .OrderBy(cs => cs.SemesterNumber)
                .ThenBy(cs => cs.SubjectVersion.Subject.SubjectCode)
                .ToListAsync();
        }
    }
}