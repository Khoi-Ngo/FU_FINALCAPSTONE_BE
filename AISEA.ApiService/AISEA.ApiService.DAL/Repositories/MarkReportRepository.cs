using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class MarkReportRepository : GenericRepository<SubjectMarkReport>
{
    public MarkReportRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SubjectMarkReport>> GetByJoinedSubjectAsync(long joinedSubjectId)
    {
        return await _context.SubjectMarkReports.Where(m => m.JoinedSubjectId == joinedSubjectId).ToListAsync();
    }
    public async Task CreateRangeAsync(IEnumerable<SubjectMarkReport> entities)
    {
        await _context.SubjectMarkReports.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}