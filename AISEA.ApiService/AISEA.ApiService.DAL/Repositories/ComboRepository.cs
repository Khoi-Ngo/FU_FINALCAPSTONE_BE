using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class ComboRepository : GenericRepository<Combo>
    {
        public ComboRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<Combo?> GetByNameAsync(string comboName)
        {
            return await _context.Combos
                .FirstOrDefaultAsync(c => c.ComboName == comboName && !c.IsDeleted);
        }

        public async Task<Combo?> GetDetailByIdAsync(long id)
        {
            return await _context.Combos
                .Include(c => c.ComboSubjects.Where(cs => !cs.IsDeleted))
                    .ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<(IEnumerable<Combo> Combos, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
        {
            var query = _context.Combos
                .Include(c => c.ComboSubjects.Where(cs => !cs.IsDeleted))
                .Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.ComboName.Contains(search) || 
                                        (c.ComboDescription != null && c.ComboDescription.Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var combos = await query
                .OrderBy(c => c.ComboName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (combos, totalCount);
        }

        public async Task<bool> IsNameUniqueAsync(string comboName, long? excludeId = null)
        {
            var query = _context.Combos.Where(c => c.ComboName == comboName && !c.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}