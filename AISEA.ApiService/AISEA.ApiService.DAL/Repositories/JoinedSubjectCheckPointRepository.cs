using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class JoinedSubjectCheckPointRepository : GenericRepository<JoinedSubjectCheckPoint>
{
    public JoinedSubjectCheckPointRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<JoinedSubjectCheckPoint> GetByIdWithJoinedSubjectAsync(long id)
    => await _context.JoinedSubjectCheckPoints.Include(c => c.JoinedSubject).FirstOrDefaultAsync(c => c.Id == id);
}