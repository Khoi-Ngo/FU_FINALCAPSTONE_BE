using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SubjectPrerequisiteRepository : GenericRepository<SubjectPrerequisite>
    {
        public SubjectPrerequisiteRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(long subjectId, long prerequisiteSubjectId)
        {
            return await _context.SubjectPrerequisites
                .AnyAsync(sp => sp.SubjectId == subjectId && sp.PrerequisiteSubjectId == prerequisiteSubjectId);
        }

        public async Task<List<Subject>> GetPrerequisitesBySubjectIdAsync(long subjectId)
        {
            return await _context.SubjectPrerequisites
                .Where(sp => sp.SubjectId == subjectId)
                .Include(sp => sp.PrerequisiteSubject)
                .Select(sp => sp.PrerequisiteSubject)
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        public async Task RemovePrerequisiteAsync(long subjectId, long prerequisiteSubjectId)
        {
            var prerequisite = await _context.SubjectPrerequisites
                .FirstOrDefaultAsync(sp => sp.SubjectId == subjectId && sp.PrerequisiteSubjectId == prerequisiteSubjectId);
            
            if (prerequisite != null)
            {
                _context.SubjectPrerequisites.Remove(prerequisite);
                await _context.SaveChangesAsync();
            }
        }
    }
}