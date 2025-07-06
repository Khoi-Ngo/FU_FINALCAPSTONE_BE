using AISEA.BgService.Worker.Abstract;
using AISEA.BgService.Worker.Entities;
using AISEA.BgService.Worker.Persistence;

namespace AISEA.BgService.DAL.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>
{
    public AuditLogRepository(AiseaContext context) : base(context)
    {
    }

    public async Task AddRangeAsync(IEnumerable<AuditLog> logs)
    {
        await _context.AuditLogs.AddRangeAsync(logs);
        await _context.SaveChangesAsync();
    }
}