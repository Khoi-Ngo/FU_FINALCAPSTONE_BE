using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class StudentProfileRepository : GenericRepository<StudentProfile>
    {
        public StudentProfileRepository(AiseaContext context) : base(context)
        {
        }
        public async Task ResetNumberOfBansAsync()
        {
            await _context.StudentProfiles
                .ExecuteUpdateAsync(setters => setters.SetProperty(sp => sp.NumberOfBan, 0));
        }
    }
}