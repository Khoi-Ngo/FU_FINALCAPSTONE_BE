using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class ComboSubjectRepository : GenericRepository<ComboSubject>
    {
        public ComboSubjectRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(long comboId, long subjectId)
        {
            return await _context.ComboSubjects
                .AnyAsync(cs => cs.ComboId == comboId && cs.SubjectId == subjectId && !cs.IsDeleted);
        }

        public async Task<List<ComboSubject>> GetByComboIdAsync(long comboId)
        {
            return await _context.ComboSubjects
                .Include(cs => cs.Subject)
                .Where(cs => cs.ComboId == comboId && !cs.IsDeleted)
                .OrderBy(cs => cs.Subject.SubjectCode)
                .ToListAsync();
        }

        public async Task RemoveSubjectFromComboAsync(long comboId, long subjectId)
        {
            var comboSubject = await _context.ComboSubjects
                .FirstOrDefaultAsync(cs => cs.ComboId == comboId && cs.SubjectId == subjectId && !cs.IsDeleted);
            
            if (comboSubject != null)
            {
                comboSubject.IsDeleted = true;
                comboSubject.DeletedAt = DateTime.UtcNow;
                await UpdateAsync(comboSubject);
            }
        }

        public async Task RemoveAllSubjectsFromComboAsync(long comboId)
        {
            var comboSubjects = await _context.ComboSubjects
                .Where(cs => cs.ComboId == comboId && !cs.IsDeleted)
                .ToListAsync();

            foreach (var comboSubject in comboSubjects)
            {
                comboSubject.IsDeleted = true;
                comboSubject.DeletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}