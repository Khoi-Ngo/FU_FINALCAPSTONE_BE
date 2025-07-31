using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class StaffProfileRepository : GenericRepository<StaffProfile>
    {
        public StaffProfileRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<long> GetUserIdByIdAsync(long id)
        {
            return await _context.StaffProfiles
                .Where(sp => sp.Id == id)
                .Select(sp => sp.UserId)
                .FirstOrDefaultAsync();
        }
    }
}