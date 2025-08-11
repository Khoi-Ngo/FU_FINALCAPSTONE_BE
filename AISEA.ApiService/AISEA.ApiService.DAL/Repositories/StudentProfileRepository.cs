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
        public async Task<long> GetUserIdByIdAsync(long id)
        {
            return await _context.StudentProfiles
                .Where(sp => sp.Id == id)
                .Select(sp => sp.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task IncreaseNumberOfBansAsync(Dictionary<long, int> studentProfileIdToBanIncrement)
        {
            foreach (var entry in studentProfileIdToBanIncrement)
            {
                var studentProfile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(sp => sp.Id == entry.Key);

                if (studentProfile != null)
                {
                    studentProfile.NumberOfBan += entry.Value;
                }
            }

            await _context.SaveChangesAsync();
        }



    }
}