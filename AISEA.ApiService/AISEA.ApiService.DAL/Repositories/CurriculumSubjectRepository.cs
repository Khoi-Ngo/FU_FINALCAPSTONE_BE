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

        public async Task<bool> ExistsAsync(long curriculumId, long subjectId)
        {
            return await _context.CurriculumSubjects
                .AnyAsync(cs => cs.CurriculumId == curriculumId && cs.SubjectId == subjectId && !cs.IsDeleted);
        }

        public async Task<List<CurriculumSubject>> GetByCurriculumIdAsync(long curriculumId)
        {
            return await _context.CurriculumSubjects
                .Include(cs => cs.Subject)
                .Where(cs => cs.CurriculumId == curriculumId && !cs.IsDeleted)
                .OrderBy(cs => cs.SemesterNumber)
                .ThenBy(cs => cs.Subject.SubjectCode)
                .ToListAsync();
        }

        public async Task RemoveSubjectFromCurriculumAsync(long curriculumId, long subjectId)
        {
            var curriculumSubject = await _context.CurriculumSubjects
                .FirstOrDefaultAsync(cs => cs.CurriculumId == curriculumId && cs.SubjectId == subjectId && !cs.IsDeleted);
            
            if (curriculumSubject != null)
            {
                curriculumSubject.IsDeleted = true;
                curriculumSubject.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}