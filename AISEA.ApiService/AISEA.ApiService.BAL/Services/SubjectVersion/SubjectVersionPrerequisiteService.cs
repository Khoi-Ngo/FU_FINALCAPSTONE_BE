using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectVersion;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.BAL.Services.SubjectVersion
{
    public class SubjectVersionPrerequisiteService
    {
        private readonly SubjectVersionRepository _subjectVersionRepository;
        private readonly SubjectVersionPrerequisiteRepository _prerequisiteRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectVersionPrerequisiteService(
            SubjectVersionRepository subjectVersionRepository,
            SubjectVersionPrerequisiteRepository prerequisiteRepository,
            SubjectRepository subjectRepository,
            IMapper mapper)
        {
            _subjectVersionRepository = subjectVersionRepository;
            _prerequisiteRepository = prerequisiteRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task AddPrerequisiteAsync(long subjectVersionId, long prerequisiteSubjectVersionId)
        {
            // Validate both subject versions exist
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(subjectVersionId);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            var prerequisiteSubjectVersion = await _subjectVersionRepository.GetByIdWithSubjectAsync(prerequisiteSubjectVersionId);
            if (prerequisiteSubjectVersion == null || prerequisiteSubjectVersion.IsDeleted)
            {
                throw new NotFoundException("Prerequisite subject version not found.");
            }

            // Ensure the Subject navigation property is loaded
            if (prerequisiteSubjectVersion.Subject == null)
            {
                throw new InvalidOperationException("Prerequisite subject version's subject information not available.");
            }

            // Validate they are not the same
            if (subjectVersionId == prerequisiteSubjectVersionId)
            {
                throw new InvalidUserCreatedException("A subject version cannot be a prerequisite of itself.");
            }

            // Check if a prerequisite with the same subject code already exists
            var hasPrerequisiteWithSameSubjectCode = await _prerequisiteRepository.HasPrerequisiteWithSubjectCodeAsync(
                subjectVersionId, prerequisiteSubjectVersion.Subject.SubjectCode);
            if (hasPrerequisiteWithSameSubjectCode)
            {
                throw new InvalidUserCreatedException(
                    $"A prerequisite with subject code '{prerequisiteSubjectVersion.Subject.SubjectCode}' already exists for this subject version.");
            }

            // Check if prerequisite was soft deleted and restore it if found
            var wasRestored = await _prerequisiteRepository.RestoreSoftDeletedPrerequisiteAsync(subjectVersionId, prerequisiteSubjectVersionId);
            if (wasRestored)
            {
                return; // Successfully restored the soft-deleted prerequisite
            }

            // Check if prerequisite already exists
            var hasPrerequisite = await _prerequisiteRepository.ExistsAsync(subjectVersionId, prerequisiteSubjectVersionId);
            if (hasPrerequisite)
            {
                throw new InvalidUserCreatedException("This prerequisite relationship already exists.");
            }

            // Check for circular dependencies
            var hasCircularDependency = await _prerequisiteRepository.HasCircularDependencyAsync(subjectVersionId, prerequisiteSubjectVersionId);
            if (hasCircularDependency)
            {
                throw new InvalidUserCreatedException("Adding this prerequisite would create a circular dependency.");
            }

            // Validate both subject versions are active
            if (!subjectVersion.IsActive || !prerequisiteSubjectVersion.IsActive)
            {
                throw new InvalidUserCreatedException("Both subject versions must be active to create prerequisite relationships.");
            }

            // Create the prerequisite relationship
            var prerequisite = new DAL.Entities.SubjectVersionPrerequisite
            {
                SubjectVersionId = subjectVersionId,
                PrerequisiteSubjectVersionId = prerequisiteSubjectVersionId,
                CreatedAt = DateTime.UtcNow
            };

            await _prerequisiteRepository.CreateAsync(prerequisite);
        }

        public async Task<List<GetSubjectVersionResponse>> GetPrerequisitesAsync(long subjectVersionId)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(subjectVersionId);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            var prerequisites = await _prerequisiteRepository.GetPrerequisitesBySubjectVersionIdAsync(subjectVersionId);
            return _mapper.Map<List<GetSubjectVersionResponse>>(prerequisites);
        }

        public async Task<List<GetSubjectVersionResponse>> GetDependentSubjectVersionsAsync(long prerequisiteSubjectVersionId)
        {
            var prerequisiteSubjectVersion = await _subjectVersionRepository.GetByIdAsync(prerequisiteSubjectVersionId);
            if (prerequisiteSubjectVersion == null || prerequisiteSubjectVersion.IsDeleted)
            {
                throw new NotFoundException("Prerequisite subject version not found.");
            }

            var dependentSubjectVersions = await _prerequisiteRepository.GetDependentSubjectVersionsByPrerequisiteIdAsync(prerequisiteSubjectVersionId);
            return _mapper.Map<List<GetSubjectVersionResponse>>(dependentSubjectVersions);
        }

        public async Task RemovePrerequisiteAsync(long subjectVersionId, long prerequisiteSubjectVersionId)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(subjectVersionId);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            await _prerequisiteRepository.RemovePrerequisiteAsync(subjectVersionId, prerequisiteSubjectVersionId);
        }

        /// <summary>
        /// Gets all prerequisites for all versions of a subject grouped by version
        /// </summary>
        public async Task<Dictionary<long, List<GetSubjectVersionResponse>>> GetPrerequisitesBySubjectIdGroupedAsync(long subjectId)
        {
            var subjectVersions = await _subjectVersionRepository.GetBySubjectIdAsync(subjectId, activeOnly: true);
            var result = new Dictionary<long, List<GetSubjectVersionResponse>>();

            foreach (var version in subjectVersions)
            {
                var prerequisites = await GetPrerequisitesAsync(version.Id);
                result[version.Id] = prerequisites;
            }

            return result;
        }

        /// <summary>
        /// Copies prerequisites from one subject version to another
        /// </summary>
        public async Task CopyPrerequisitesAsync(long fromSubjectVersionId, long toSubjectVersionId)
        {
            var fromVersion = await _subjectVersionRepository.GetByIdAsync(fromSubjectVersionId);
            var toVersion = await _subjectVersionRepository.GetByIdAsync(toSubjectVersionId);

            if (fromVersion == null || fromVersion.IsDeleted)
            {
                throw new NotFoundException("Source subject version not found.");
            }

            if (toVersion == null || toVersion.IsDeleted)
            {
                throw new NotFoundException("Target subject version not found.");
            }

            var prerequisites = await _prerequisiteRepository.GetPrerequisitesBySubjectVersionIdAsync(fromSubjectVersionId);

            foreach (var prerequisite in prerequisites)
            {
                try
                {
                    await AddPrerequisiteAsync(toSubjectVersionId, prerequisite.Id);
                }
                catch (InvalidUserCreatedException)
                {
                    // Skip prerequisites that would create conflicts or circular dependencies
                    continue;
                }
            }
        }

        /// <summary>
        /// Gets all prerequisites for a subject based on its subject code
        /// Returns unique prerequisites across all active versions of the subject
        /// </summary>
        public async Task<List<GetSubjectVersionResponse>> GetPrerequisitesBySubjectCodeAsync(string subjectCode)
        {
            var subject = await _subjectRepository.GetByCodeAsync(subjectCode);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException($"Subject with code '{subjectCode}' not found.");
            }

            var subjectVersions = await _subjectVersionRepository.GetBySubjectIdAsync(subject.Id, activeOnly: true);
            if (!subjectVersions.Any())
            {
                return new List<GetSubjectVersionResponse>();
            }

            var allPrerequisites = new List<DAL.Entities.SubjectVersion>();
            var uniqueSubjectCodes = new HashSet<string>();

            // Get prerequisites from all active versions of the subject
            foreach (var version in subjectVersions)
            {
                var versionPrerequisites = await _prerequisiteRepository.GetPrerequisitesBySubjectVersionIdAsync(version.Id);
                
                // Add unique prerequisites (avoid duplicates based on subject code)
                foreach (var prerequisite in versionPrerequisites)
                {
                    if (uniqueSubjectCodes.Add(prerequisite.Subject.SubjectCode))
                    {
                        allPrerequisites.Add(prerequisite);
                    }
                }
            }

            return _mapper.Map<List<GetSubjectVersionResponse>>(allPrerequisites);
        }
    }
}
