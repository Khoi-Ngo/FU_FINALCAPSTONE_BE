using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SyllabusRepository : GenericRepository<Syllabus>
    {
        public SyllabusRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<Syllabus?> GetBySubjectIdAsync(long subjectId)
        {
            return await _context.Syllabi
                .Include(s => s.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .FirstOrDefaultAsync(s => s.SubjectVersion.SubjectId == subjectId && !s.IsDeleted);
        }
        
        public async Task<Syllabus?> GetBySubjectVersionIdAsync(long subjectVersionId)
        {
            return await _context.Syllabi
                .Include(s => s.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .FirstOrDefaultAsync(s => s.SubjectVersionId == subjectVersionId && !s.IsDeleted);
        }

        public async Task<Syllabus?> GetDetailByIdAsync(long id)
        {
            return await _context.Syllabi
                .Include(s => s.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Include(s => s.SyllabusAssessments.Where(a => !a.IsDeleted))
                .Include(s => s.SyllabusLearningMaterials.Where(m => !m.IsDeleted))
                .Include(s => s.SyllabusLearningOutcomes.Where(o => !o.IsDeleted))
                .Include(s => s.SyllabusSessions.Where(sess => !sess.IsDeleted))
                    .ThenInclude(sess => sess.SessionOutcomeMappings)
                    .ThenInclude(som => som.Outcome)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<(IEnumerable<Syllabus> Syllabi, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Syllabi
                .Include(s => s.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Where(s => !s.IsDeleted);

            var totalCount = await query.CountAsync();
            var syllabi = await query
                .OrderBy(s => s.SubjectVersion.Subject.SubjectCode)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (syllabi, totalCount);
        }
    }
}