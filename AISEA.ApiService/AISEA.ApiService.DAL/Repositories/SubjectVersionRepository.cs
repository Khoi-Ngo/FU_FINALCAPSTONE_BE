using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SubjectVersionRepository : GenericRepository<SubjectVersion>
    {
        public SubjectVersionRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<SubjectVersion?> GetBySubjectIdAndVersionCodeAsync(long subjectId, string versionCode)
        {
            return await _context.SubjectVersions
                .Include(sv => sv.Subject)
                .FirstOrDefaultAsync(sv => sv.SubjectId == subjectId && 
                                          sv.VersionCode == versionCode && 
                                          !sv.IsDeleted);
        }

        public async Task<(IEnumerable<SubjectVersion> Versions, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, long? subjectId = null, string? search = null, bool? isActive = null)
        {
            var query = _context.SubjectVersions
                .Include(sv => sv.Subject)
                .Where(sv => !sv.IsDeleted);

            if (subjectId.HasValue)
            {
                query = query.Where(sv => sv.SubjectId == subjectId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(sv => sv.IsActive == isActive.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(sv => sv.VersionName.Contains(search) || 
                                         sv.VersionCode.Contains(search) ||
                                         sv.Subject.SubjectName.Contains(search) ||
                                         sv.Subject.SubjectCode.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var versions = await query
                .OrderByDescending(sv => sv.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (versions, totalCount);
        }

        public async Task<List<SubjectVersion>> GetBySubjectIdAsync(long subjectId, bool activeOnly = false)
        {
            var query = _context.SubjectVersions
                .Include(sv => sv.Subject)
                .Where(sv => sv.SubjectId == subjectId && !sv.IsDeleted);

            if (activeOnly)
            {
                query = query.Where(sv => sv.IsActive);
            }

            return await query
                .OrderByDescending(sv => sv.CreatedAt)
                .ToListAsync();
        }

        public async Task<SubjectVersion?> GetDefaultVersionAsync(long subjectId)
        {
            return await _context.SubjectVersions
                .Include(sv => sv.Subject)
                .FirstOrDefaultAsync(sv => sv.SubjectId == subjectId && 
                                          sv.IsDefault && 
                                          sv.IsActive && 
                                          !sv.IsDeleted);
        }

        public async Task<List<SubjectVersion>> GetActiveVersionsAsync(DateTime? asOfDate = null)
        {
            var targetDate = asOfDate ?? DateTime.UtcNow;
            
            return await _context.SubjectVersions
                .Include(sv => sv.Subject)
                .Where(sv => sv.IsActive && 
                            !sv.IsDeleted &&
                            sv.EffectiveFrom <= targetDate &&
                            (sv.EffectiveTo == null || sv.EffectiveTo >= targetDate))
                .OrderBy(sv => sv.Subject.SubjectCode)
                .ThenByDescending(sv => sv.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasActiveVersionAsync(long subjectId)
        {
            return await _context.SubjectVersions
                .AnyAsync(sv => sv.SubjectId == subjectId && sv.IsActive && !sv.IsDeleted);
        }

        public async Task<bool> HasOtherActiveVersionsAsync(long subjectId, long excludeVersionId)
        {
            return await _context.SubjectVersions
                .AnyAsync(sv => sv.SubjectId == subjectId && 
                               sv.Id != excludeVersionId && 
                               sv.IsActive && 
                               !sv.IsDeleted);
        }

        public async Task<bool> ExistsAsync(long subjectId, string versionCode, long? excludeId = null)
        {
            var query = _context.SubjectVersions
                .Where(sv => sv.SubjectId == subjectId && 
                            sv.VersionCode == versionCode && 
                            !sv.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(sv => sv.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public new async Task<SubjectVersion?> GetByIdAsync(long id)
        {
            return await _context.SubjectVersions
                .Include(sv => sv.Subject)
                .FirstOrDefaultAsync(sv => sv.Id == id && !sv.IsDeleted);
        }

        /// <summary>
        /// Creates a new subject version and handles default version logic atomically
        /// </summary>
        public async Task CreateSubjectVersionWithDefaultHandlingAsync(SubjectVersion newVersion, bool isDefault, long subjectId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // If this is set as default, clear the current default first
                if (isDefault)
                {
                    var currentDefault = await _context.SubjectVersions
                        .FirstOrDefaultAsync(sv => sv.SubjectId == subjectId && 
                                                  sv.IsDefault && 
                                                  sv.IsActive && 
                                                  !sv.IsDeleted);
                    if (currentDefault != null)
                    {
                        currentDefault.IsDefault = false;
                        currentDefault.UpdatedAt = DateTime.UtcNow;
                        _context.Update(currentDefault);
                    }
                }

                // Create the new version
                _context.Add(newVersion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Updates a subject version and handles default version logic atomically
        /// </summary>
        public async Task UpdateSubjectVersionWithDefaultHandlingAsync(SubjectVersion versionToUpdate, bool setAsDefault)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // If this is being set as default, clear the current default first
                if (setAsDefault && !versionToUpdate.IsDefault)
                {
                    var currentDefault = await _context.SubjectVersions
                        .FirstOrDefaultAsync(sv => sv.SubjectId == versionToUpdate.SubjectId && 
                                                  sv.IsDefault && 
                                                  sv.IsActive && 
                                                  !sv.IsDeleted);
                    if (currentDefault != null)
                    {
                        currentDefault.IsDefault = false;
                        currentDefault.UpdatedAt = DateTime.UtcNow;
                        _context.Update(currentDefault);
                    }
                }

                // Update the target version
                _context.Update(versionToUpdate);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Sets a version as default and handles clearing the previous default atomically
        /// </summary>
        public async Task SetDefaultVersionAsync(long versionId, long subjectId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get the version to set as default
                var versionToSetAsDefault = await _context.SubjectVersions
                    .FirstOrDefaultAsync(sv => sv.Id == versionId && !sv.IsDeleted);
                
                if (versionToSetAsDefault == null)
                    throw new InvalidOperationException("Subject version not found.");

                // Clear current default
                var currentDefault = await _context.SubjectVersions
                    .FirstOrDefaultAsync(sv => sv.SubjectId == subjectId && 
                                              sv.IsDefault && 
                                              sv.IsActive && 
                                              !sv.IsDeleted);
                
                if (currentDefault != null && currentDefault.Id != versionId)
                {
                    currentDefault.IsDefault = false;
                    currentDefault.UpdatedAt = DateTime.UtcNow;
                    _context.Update(currentDefault);
                }

                // Set new default
                versionToSetAsDefault.IsDefault = true;
                versionToSetAsDefault.UpdatedAt = DateTime.UtcNow;
                _context.Update(versionToSetAsDefault);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
