using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectVersion;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.SubjectVersion
{
    public class SubjectVersionService
    {
        private readonly SubjectVersionRepository _subjectVersionRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectVersionService(
            SubjectVersionRepository subjectVersionRepository,
            SubjectRepository subjectRepository,
            IMapper mapper)
        {
            _subjectVersionRepository = subjectVersionRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task CreateSubjectVersionAsync(CreateSubjectVersionRequest request)
        {
            // Validate subject exists
            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            // Check if version code already exists for this subject
            var existingVersion = await _subjectVersionRepository.GetBySubjectIdAndVersionCodeAsync(
                request.SubjectId, request.VersionCode);
            if (existingVersion != null)
            {
                throw new InvalidUserCreatedException(
                    $"Version '{request.VersionCode}' already exists for this subject.");
            }

            // If this is set as default, ensure no other version is default for this subject
            if (request.IsDefault)
            {
                var currentDefault = await _subjectVersionRepository.GetDefaultVersionAsync(request.SubjectId);
                if (currentDefault != null)
                {
                    currentDefault.IsDefault = false;
                    currentDefault.UpdatedAt = DateTime.UtcNow;
                    await _subjectVersionRepository.UpdateAsync(currentDefault);
                }
            }

            var subjectVersion = _mapper.Map<DAL.Entities.SubjectVersion>(request);
            subjectVersion.CreatedAt = DateTime.UtcNow;

            await _subjectVersionRepository.CreateAsync(subjectVersion);
        }

        public async Task<PagedResult<GetSubjectVersionResponse>> GetSubjectVersionsPagedAsync(
            PaginationRequest request, long? subjectId = null, string? search = null, bool? isActive = null)
        {
            var (versions, totalCount) = await _subjectVersionRepository.GetPagedAsync(
                request.PageNumber, request.PageSize, subjectId, search, isActive);

            return new PagedResult<GetSubjectVersionResponse>
            {
                Items = _mapper.Map<List<GetSubjectVersionResponse>>(versions),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetSubjectVersionResponse> GetSubjectVersionByIdAsync(long id)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(id);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            return _mapper.Map<GetSubjectVersionResponse>(subjectVersion);
        }

        public async Task<List<GetSubjectVersionResponse>> GetSubjectVersionsBySubjectIdAsync(
            long subjectId, bool activeOnly = false)
        {
            // Validate subject exists
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            var versions = await _subjectVersionRepository.GetBySubjectIdAsync(subjectId, activeOnly);
            return _mapper.Map<List<GetSubjectVersionResponse>>(versions);
        }

        public async Task<GetSubjectVersionResponse?> GetDefaultVersionAsync(long subjectId)
        {
            // Validate subject exists
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            var defaultVersion = await _subjectVersionRepository.GetDefaultVersionAsync(subjectId);
            return defaultVersion != null ? _mapper.Map<GetSubjectVersionResponse>(defaultVersion) : null;
        }

        public async Task<List<GetSubjectVersionResponse>> GetActiveVersionsAsync(DateTime? asOfDate = null)
        {
            var activeVersions = await _subjectVersionRepository.GetActiveVersionsAsync(asOfDate);
            return _mapper.Map<List<GetSubjectVersionResponse>>(activeVersions);
        }

        public async Task UpdateSubjectVersionAsync(long id, UpdateSubjectVersionRequest request)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(id);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            // Check if version code is being changed and if it conflicts with existing
            if (subjectVersion.VersionCode != request.VersionCode)
            {
                var existingVersion = await _subjectVersionRepository.ExistsAsync(
                    subjectVersion.SubjectId, request.VersionCode, id);
                if (existingVersion)
                {
                    throw new InvalidUserCreatedException(
                        $"Version '{request.VersionCode}' already exists for this subject.");
                }
            }

            // If this is being set as default, ensure no other version is default for this subject
            if (request.IsDefault && !subjectVersion.IsDefault)
            {
                var currentDefault = await _subjectVersionRepository.GetDefaultVersionAsync(subjectVersion.SubjectId);
                if (currentDefault != null)
                {
                    currentDefault.IsDefault = false;
                    currentDefault.UpdatedAt = DateTime.UtcNow;
                    await _subjectVersionRepository.UpdateAsync(currentDefault);
                }
            }

            _mapper.Map(request, subjectVersion);
            subjectVersion.UpdatedAt = DateTime.UtcNow;

            await _subjectVersionRepository.UpdateAsync(subjectVersion);
        }

        public async Task DeleteSubjectVersionAsync(long id)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(id);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            // Check if this is the only version for the subject
            var hasOtherActiveVersions = await _subjectVersionRepository.HasActiveVersionAsync(subjectVersion.SubjectId);
            if (!hasOtherActiveVersions && subjectVersion.IsActive)
            {
                throw new InvalidOperationException(
                    "Cannot delete the only active version of a subject. Create another version first.");
            }

            subjectVersion.IsDeleted = true;
            subjectVersion.DeletedAt = DateTime.UtcNow;

            await _subjectVersionRepository.UpdateAsync(subjectVersion);
        }

        public async Task SetDefaultVersionAsync(long id)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(id);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            if (!subjectVersion.IsActive)
            {
                throw new InvalidOperationException("Cannot set an inactive version as default.");
            }

            // Remove default flag from current default version
            var currentDefault = await _subjectVersionRepository.GetDefaultVersionAsync(subjectVersion.SubjectId);
            if (currentDefault != null && currentDefault.Id != id)
            {
                currentDefault.IsDefault = false;
                currentDefault.UpdatedAt = DateTime.UtcNow;
                await _subjectVersionRepository.UpdateAsync(currentDefault);
            }

            // Set new default
            subjectVersion.IsDefault = true;
            subjectVersion.UpdatedAt = DateTime.UtcNow;
            await _subjectVersionRepository.UpdateAsync(subjectVersion);
        }

        public async Task ToggleActiveStatusAsync(long id)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(id);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            // If deactivating, check if it's the default version
            if (subjectVersion.IsActive && subjectVersion.IsDefault)
            {
                throw new InvalidOperationException(
                    "Cannot deactivate the default version. Set another version as default first.");
            }

            subjectVersion.IsActive = !subjectVersion.IsActive;
            subjectVersion.UpdatedAt = DateTime.UtcNow;

            await _subjectVersionRepository.UpdateAsync(subjectVersion);
        }
    }
}
