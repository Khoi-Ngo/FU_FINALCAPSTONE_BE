using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class SemesterRepository : GenericRepository<Semester>
{
    public SemesterRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<bool> SemesterExistsAsync(string semesterName)
    {
        return await _context.Semesters.AnyAsync(s => s.SemesterName == semesterName);
    }
}