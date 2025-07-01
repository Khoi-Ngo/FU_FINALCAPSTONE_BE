using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SyllabusAssessmentRepository : GenericRepository<SyllabusAssessment>
    {
        public SyllabusAssessmentRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<List<SyllabusAssessment>> GetBySyllabusIdAsync(long syllabusId)
        {
            return await _context.SyllabusAssessments
                .Where(sa => sa.SyllabusId == syllabusId && !sa.IsDeleted)
                .OrderBy(sa => sa.Category)
                .ToListAsync();
        }
    }
}