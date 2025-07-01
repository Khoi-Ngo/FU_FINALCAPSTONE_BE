using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SyllabusLearningMaterialRepository : GenericRepository<SyllabusLearningMaterial>
    {
        public SyllabusLearningMaterialRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<List<SyllabusLearningMaterial>> GetBySyllabusIdAsync(long syllabusId)
        {
            return await _context.SyllabusLearningMaterials
                .Where(slm => slm.SyllabusId == syllabusId && !slm.IsDeleted)
                .OrderBy(slm => slm.MaterialName)
                .ToListAsync();
        }
    }
}