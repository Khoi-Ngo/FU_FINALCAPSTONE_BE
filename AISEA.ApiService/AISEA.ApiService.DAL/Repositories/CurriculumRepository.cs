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
                .FirstOrDefaultAsync(c => c.CurriculumCode == curriculumCode && !c.IsDeleted);
        }

        public async Task<(IEnumerable<Curriculum> Curricula, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            string? search = null,
            long? programId = null,
            DateTimeOffset? effectiveDateFrom = null,
            DateTimeOffset? effectiveDateTo = null,
            bool? isActive = null,
            string? sortBy = "CurriculumName",
            string? sortOrder = "asc")
        {
            var query = _context.Curricula
                .Include(c => c.Program)
                .Include(c => c.CurriculumSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CurriculumName.Contains(search) || 
                                       c.CurriculumCode.Contains(search) ||
                                       c.Program.ProgramName.Contains(search));
            }

            if (programId.HasValue)
            {
                query = query.Where(c => c.ProgramId == programId.Value);
            }

            if (effectiveDateFrom.HasValue)
            {
                query = query.Where(c => c.EffectiveDate >= effectiveDateFrom.Value);
            }

            if (effectiveDateTo.HasValue)
            {
                query = query.Where(c => c.EffectiveDate <= effectiveDateTo.Value);
            }

            if (isActive.HasValue)
            {
                var currentDate = DateTimeOffset.UtcNow;
                if (isActive.Value)
                {
                    query = query.Where(c => c.EffectiveDate <= currentDate);
                }
                else
                {
                    query = query.Where(c => c.EffectiveDate > currentDate);
                }
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "curriculumcode" => sortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(c => c.CurriculumCode)
                    : query.OrderBy(c => c.CurriculumCode),
                "effectivedate" => sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.EffectiveDate)
                    : query.OrderBy(c => c.EffectiveDate),
                "programname" => sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Program.ProgramName)
                    : query.OrderBy(c => c.Program.ProgramName),
                _ => sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.CurriculumName)
                    : query.OrderBy(c => c.CurriculumName)
            };

            var totalCount = await query.CountAsync();
            var curricula = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (curricula, totalCount);
        }

        public async Task<Curriculum?> GetDetailByIdAsync(long id)
        {
            return await _context.Curricula
                .Include(c => c.Program)
                .Include(c => c.CurriculumSubjects.Where(cs => !cs.IsDeleted))
                    .ThenInclude(cs => cs.Subject)
                        .ThenInclude(s => s.Prerequisites)
                            .ThenInclude(p => p.PrerequisiteSubject)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<List<Curriculum>> GetActiveCurriculaAsync()
        {
            var currentDate = DateTimeOffset.UtcNow;
            return await _context.Curricula
                .Include(c => c.Program)
                .Where(c => !c.IsDeleted && c.EffectiveDate <= currentDate)
                .OrderBy(c => c.CurriculumName)
                .ToListAsync();
        }
    }
}