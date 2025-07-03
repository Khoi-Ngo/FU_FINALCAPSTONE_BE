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
                       (isStudentQuery ? s.StudentId == profileId : s.StaffId == profileId)).OrderByDescending(s => s.CreatedAt).OrderByDescending(s => s.UpdatedAt);

            var totalCountTask = query.CountAsync();
            var sessionsTask = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            await Task.WhenAll(totalCountTask, sessionsTask);

            return (sessionsTask.Result, totalCountTask.Result);
        }


    }
}