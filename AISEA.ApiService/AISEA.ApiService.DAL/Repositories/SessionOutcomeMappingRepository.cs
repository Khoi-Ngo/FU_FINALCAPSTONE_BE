using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SessionOutcomeMappingRepository : GenericRepository<SessionOutcomeMapping>
    {
        public SessionOutcomeMappingRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(long sessionId, long outcomeId)
        {
            return await _context.SessionOutcomeMappings
                .AnyAsync(som => som.SessionId == sessionId && som.OutcomeId == outcomeId && !som.IsDeleted);
        }

        public async Task<List<SessionOutcomeMapping>> GetBySessionIdAsync(long sessionId)
        {
            return await _context.SessionOutcomeMappings
                .Include(som => som.Outcome)
                .Where(som => som.SessionId == sessionId && !som.IsDeleted)
                .ToListAsync();
        }

        public async Task<SessionOutcomeMapping?> GetBySessionIdAndOutcomeIdAsync(long sessionId, long outcomeId)
        {
            return await _context.SessionOutcomeMappings
                .FirstOrDefaultAsync(som => som.SessionId == sessionId && som.OutcomeId == outcomeId && !som.IsDeleted);
        }

        public async Task<SessionOutcomeMapping?> GetDeletedMappingAsync(long sessionId, long outcomeId)
        {
            return await _context.SessionOutcomeMappings
                .FirstOrDefaultAsync(som => som.SessionId == sessionId && som.OutcomeId == outcomeId && som.IsDeleted);
        }
    }
}