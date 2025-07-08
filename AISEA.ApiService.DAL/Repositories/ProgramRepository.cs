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
        
        public async Task<(IEnumerable<Program> Programs, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
        {
            var query = _context.Programs.Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProgramName.Contains(search) || p.ProgramCode.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var programs = await query
                .OrderBy(p => p.ProgramCode)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (programs, totalCount);
        }
        
        public async Task<bool> HasCurriculaAsync(long programId)
        {
            return await _context.Curricula
                .AnyAsync(c => c.ProgramId == programId && !c.IsDeleted);
        }
    }
}