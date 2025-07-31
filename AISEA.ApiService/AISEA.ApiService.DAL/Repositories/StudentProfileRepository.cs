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
            var updates = studentProfileIdToBanIncrement
                .Select(entry => new { StudentProfileId = entry.Key, Increment = entry.Value })
                .ToList();

            foreach (var batch in updates.Chunk(1000))
            {
                var studentIds = batch.Select(u => u.StudentProfileId).ToList();
                await _context.StudentProfiles
                    .Where(sp => studentIds.Contains(sp.Id))
                    .ExecuteUpdateAsync(setters => setters.SetProperty(
                        sp => sp.NumberOfBan,
                        sp => sp.NumberOfBan + batch.First(b => b.StudentProfileId == sp.Id).Increment));
            }
        }
    }
}