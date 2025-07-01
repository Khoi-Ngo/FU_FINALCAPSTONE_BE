using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class ProgramRepository : GenericRepository<Program>
    {
        public ProgramRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<Program?> GetByCodeAsync(string programCode)
        {
            return await _context.Programs
                .FirstOrDefaultAsync(p => p.ProgramCode == programCode && !p.IsDeleted);
        }

        public async Task<List<Program>> GetAllActiveAsync()
        {
            return await _context.Programs
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.ProgramCode)
                .ToListAsync();
        }
    }
}