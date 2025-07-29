using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SubjectVersionPrerequisiteRepository : GenericRepository<SubjectVersionPrerequisite>
    {
        public SubjectVersionPrerequisiteRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(long subjectVersionId, long prerequisiteSubjectVersionId)
        {
            return await _context.SubjectVersionPrerequisites
                .AnyAsync(svp => svp.SubjectVersionId == subjectVersionId && 
                                svp.PrerequisiteSubjectVersionId == prerequisiteSubjectVersionId &&
                                !svp.IsDeleted);
        }

        public async Task<List<SubjectVersion>> GetPrerequisitesBySubjectVersionIdAsync(long subjectVersionId)
        {
            return await _context.SubjectVersionPrerequisites
                .Where(svp => svp.SubjectVersionId == subjectVersionId && !svp.IsDeleted)
                .Include(svp => svp.PrerequisiteSubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Select(svp => svp.PrerequisiteSubjectVersion)
                .Where(sv => !sv.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<SubjectVersion>> GetDependentSubjectVersionsByPrerequisiteIdAsync(long prerequisiteSubjectVersionId)
        {
            return await _context.SubjectVersionPrerequisites
                .Where(svp => svp.PrerequisiteSubjectVersionId == prerequisiteSubjectVersionId && !svp.IsDeleted)
                .Include(svp => svp.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Select(svp => svp.SubjectVersion)
                .Where(sv => !sv.IsDeleted)
                .ToListAsync();
        }

        public async Task RemovePrerequisiteAsync(long subjectVersionId, long prerequisiteSubjectVersionId)
        {
            var prerequisite = await _context.SubjectVersionPrerequisites
                .FirstOrDefaultAsync(svp => svp.SubjectVersionId == subjectVersionId && 
                                           svp.PrerequisiteSubjectVersionId == prerequisiteSubjectVersionId &&
                                           !svp.IsDeleted);
            
            if (prerequisite != null)
            {
                prerequisite.IsDeleted = true;
                prerequisite.DeletedAt = DateTime.UtcNow;
                _context.SubjectVersionPrerequisites.Update(prerequisite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SubjectVersionPrerequisite>> GetPrerequisitesBySubjectIdAsync(long subjectId)
        {
            return await _context.SubjectVersionPrerequisites
                .Include(svp => svp.SubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Include(svp => svp.PrerequisiteSubjectVersion)
                    .ThenInclude(sv => sv.Subject)
                .Where(svp => svp.SubjectVersion.SubjectId == subjectId && !svp.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> HasCircularDependencyAsync(long subjectVersionId, long prerequisiteSubjectVersionId)
        {
            // Check if adding this prerequisite would create a circular dependency
            // This is a simple check - you might want to implement a more comprehensive graph traversal
            var existingPrerequisites = await GetPrerequisitesBySubjectVersionIdAsync(prerequisiteSubjectVersionId);
            return existingPrerequisites.Any(p => p.Id == subjectVersionId);
        }

        public async Task<bool> RestoreSoftDeletedPrerequisiteAsync(long subjectVersionId, long prerequisiteSubjectVersionId)
        {
            var softDeletedPrerequisite = await _context.SubjectVersionPrerequisites
                .FirstOrDefaultAsync(svp => svp.SubjectVersionId == subjectVersionId &&
                                           svp.PrerequisiteSubjectVersionId == prerequisiteSubjectVersionId &&
                                           svp.IsDeleted);

            if (softDeletedPrerequisite != null)
            {
                softDeletedPrerequisite.IsDeleted = false;
                softDeletedPrerequisite.UpdatedAt = DateTime.UtcNow;
                _context.SubjectVersionPrerequisites.Update(softDeletedPrerequisite);
                await _context.SaveChangesAsync();
                return true; // Successfully restored
            }

            return false; // No soft-deleted prerequisite found
        }
    }
}
