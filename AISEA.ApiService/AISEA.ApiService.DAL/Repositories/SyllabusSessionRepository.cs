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

        /// <summary>
        /// Checks if a session number already exists within a syllabus
        /// </summary>
        public async Task<bool> ExistsSessionNumberAsync(long syllabusId, int sessionNumber)
        {
            return await _context.SyllabusSessions
                .AnyAsync(ss => ss.SyllabusId == syllabusId && 
                               ss.SessionNumber == sessionNumber && 
                               !ss.IsDeleted);
        }

        /// <summary>
        /// Checks if a session number already exists within a syllabus, excluding a specific session ID
        /// </summary>
        public async Task<bool> ExistsSessionNumberAsync(long syllabusId, int sessionNumber, long excludeSessionId)
        {
            return await _context.SyllabusSessions
                .AnyAsync(ss => ss.SyllabusId == syllabusId && 
                               ss.SessionNumber == sessionNumber && 
                               ss.Id != excludeSessionId &&
                               !ss.IsDeleted);
        }
    }
}