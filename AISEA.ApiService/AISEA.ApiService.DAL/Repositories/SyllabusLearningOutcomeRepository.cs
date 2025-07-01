using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SyllabusLearningOutcomeRepository : GenericRepository<SyllabusLearningOutcome>
    {
        public SyllabusLearningOutcomeRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<List<SyllabusLearningOutcome>> GetBySyllabusIdAsync(long syllabusId)
        {
            return await _context.SyllabusLearningOutcomes
                .Where(slo => slo.SyllabusId == syllabusId && !slo.IsDeleted)
                .OrderBy(slo => slo.OutcomeCode)
                .ToListAsync();
        }

        public async Task<SyllabusLearningOutcome?> GetByCodeAsync(long syllabusId, string outcomeCode)
        {
            return await _context.SyllabusLearningOutcomes
                .FirstOrDefaultAsync(slo => slo.SyllabusId == syllabusId && slo.OutcomeCode == outcomeCode && !slo.IsDeleted);
        }
    }
}