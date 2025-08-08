using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SyllabusSessionRepository : GenericRepository<SyllabusSession>
    {
        public SyllabusSessionRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<List<SyllabusSession>> GetBySyllabusIdAsync(long syllabusId)
        {
            return await _context.SyllabusSessions
                .Include(ss => ss.SessionOutcomeMappings.Where(som => !som.IsDeleted))
                    .ThenInclude(som => som.Outcome)
                .Where(ss => ss.SyllabusId == syllabusId && !ss.IsDeleted)
                .OrderBy(ss => ss.SessionNumber)
                .ToListAsync();
        }
    }
}