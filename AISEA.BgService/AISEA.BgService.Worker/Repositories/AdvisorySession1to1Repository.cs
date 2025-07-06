using AISEA.BgService.Worker.Abstract;
using AISEA.BgService.Worker.Entities;
using AISEA.BgService.Worker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.BgService.Worker.Repositories
{
    public class AdvisorySession1to1Repository : GenericRepository<AdvisorySession1to1>
    {
        public AdvisorySession1to1Repository(AiseaContext context) : base(context)
        {
        }
        public async Task<List<long>> RemoveAllExistedOverDaysAsync(int sessionExpiryDays)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(-sessionExpiryDays);

            var sessionsToRemove = await _context.AdvisorySessions1to1
                .Where(s => s.CreatedAt < thresholdDate)
                .ToListAsync();

            var sessionIds = sessionsToRemove.Select(s => s.Id).ToList();

            _context.AdvisorySessions1to1.RemoveRange(sessionsToRemove);

            await _context.SaveChangesAsync();

            return sessionIds;
        }
    }
}