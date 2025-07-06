using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class AdvisorySession1to1Repository : GenericRepository<AdvisorySession1to1>
    {
        public AdvisorySession1to1Repository(AiseaContext context) : base(context)
        {
        }

        public async Task<(List<AdvisorySession1to1> sessions, int totalCount)> GetSessionsByProfileId(
                        int pageNumber, int pageSize, bool isStudentQuery,
                        EAdvisorySession1to1Type sessionType, long profileId)
        {
            var query = _context.AdvisorySessions1to1
                .Where(s => s.Type == sessionType &&
                            (isStudentQuery ? s.StudentId == profileId : s.StaffId == profileId))
                .OrderByDescending(s => s.CreatedAt)
                .OrderByDescending(s => s.UpdatedAt);

            // Execute queries sequentially
            var totalCount = await query.CountAsync();
            var sessions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (sessions, totalCount);
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