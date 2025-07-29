using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
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
    }
}