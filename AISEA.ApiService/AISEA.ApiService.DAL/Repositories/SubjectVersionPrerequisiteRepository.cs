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
            // If adding prerequisiteSubjectVersionId as a prerequisite of subjectVersionId would create a cycle,
            // then there must be a path from prerequisiteSubjectVersionId back to subjectVersionId
            return await HasPathAsync(prerequisiteSubjectVersionId, subjectVersionId, new HashSet<long>());
        }

        /// <summary>
        /// Checks if there's a path from startSubjectVersionId to targetSubjectVersionId using DFS
        /// This detects both direct and transitive circular dependencies
        /// </summary>
        /// <param name="startSubjectVersionId">Starting subject version ID</param>
        /// <param name="targetSubjectVersionId">Target subject version ID we're looking for</param>
        /// <param name="visited">Set of already visited nodes to prevent infinite loops</param>
        /// <returns>True if a path exists, false otherwise</returns>
        private async Task<bool> HasPathAsync(long startSubjectVersionId, long targetSubjectVersionId, HashSet<long> visited)
        {
            // If we've already visited this node, there's no cycle through this path
            if (visited.Contains(startSubjectVersionId))
            {
                return false;
            }

            // If we've reached our target, we found a path (potential cycle)
            if (startSubjectVersionId == targetSubjectVersionId)
            {
                return true;
            }

            // Mark this node as visited
            visited.Add(startSubjectVersionId);

            try
            {
                // Get all prerequisites of the current subject version
                var prerequisites = await GetPrerequisitesBySubjectVersionIdAsync(startSubjectVersionId);

                // For each prerequisite, recursively check if there's a path to the target
                foreach (var prerequisite in prerequisites)
                {
                    if (await HasPathAsync(prerequisite.Id, targetSubjectVersionId, new HashSet<long>(visited)))
                    {
                        return true; // Found a path through this prerequisite
                    }
                }

                return false; // No path found through any prerequisite
            }
            finally
            {
                // Remove from visited set to allow other paths to explore this node
                visited.Remove(startSubjectVersionId);
            }
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
