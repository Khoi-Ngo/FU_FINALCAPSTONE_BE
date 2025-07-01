using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Curriculum;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Curriculum;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Curriculum
{
    public class CurriculumService
    {
        private readonly CurriculumRepository _curriculumRepository;
        private readonly CurriculumVersionRepository _versionRepository;
        private readonly CurriculumSubjectRepository _curriculumSubjectRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public CurriculumService(
            CurriculumRepository curriculumRepository,
            CurriculumVersionRepository versionRepository,
            CurriculumSubjectRepository curriculumSubjectRepository,
            SubjectRepository subjectRepository,
            IMapper mapper)
        {
            _curriculumRepository = curriculumRepository;
            _versionRepository = versionRepository;
            _curriculumSubjectRepository = curriculumSubjectRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateCurriculumAsync(CreateCurriculumRequest request)
        {
            // Check if curriculum code already exists
            var existingCurriculum = await _curriculumRepository.GetByCodeAsync(request.CurriculumCode);
            if (existingCurriculum != null)
            {
                throw new InvalidUserCreatedException($"Curriculum with code '{request.CurriculumCode}' already exists.");
            }

            var curriculum = _mapper.Map<DAL.Entities.Curriculum>(request);
            curriculum.CreatedAt = DateTime.UtcNow;
            
            await _curriculumRepository.CreateAsync(curriculum);

            // Create initial version
            var version = new DAL.Entities.CurriculumVersion
            {
                CurriculumId = curriculum.Id,
                Version = "1.0",
                EffectiveDate = request.EffectiveDate,
                ChangeDescription = "Initial version",
                CreatedAt = DateTime.UtcNow
            };
            await _versionRepository.CreateAsync(version);

            // Add subjects if provided
            if (request.Subjects?.Any() == true)
            {
                await AddSubjectsToCurriculumAsync(curriculum.Id, request.Subjects);
            }

            return curriculum.Id;
        }

        public async Task<PagedResult<GetCurriculumResponse>> GetCurriculaPagedAsync(CurriculumSearchRequest request)
        {
            var (curricula, totalCount) = await _curriculumRepository.GetPagedAsync(
                request.PageNumber, 
                request.PageSize, 
                request.Search,
                request.ProgramId,
                request.EffectiveDateFrom,
                request.EffectiveDateTo,
                request.IsActive,
                request.SortBy,
                request.SortOrder);
            
            return new PagedResult<GetCurriculumResponse>
            {
                Items = _mapper.Map<List<GetCurriculumResponse>>(curricula),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetCurriculumDetailResponse> GetCurriculumDetailAsync(long id)
        {
            var curriculum = await _curriculumRepository.GetDetailByIdAsync(id);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            return _mapper.Map<GetCurriculumDetailResponse>(curriculum);
        }

        public async Task UpdateCurriculumAsync(long id, UpdateCurriculumRequest request)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            // Create new version if there are significant changes
            var hasSignificantChanges = curriculum.CurriculumName != request.CurriculumName ||
                                      curriculum.EffectiveDate != request.EffectiveDate;

            _mapper.Map(request, curriculum);
            curriculum.UpdatedAt = DateTime.UtcNow;
            
            await _curriculumRepository.UpdateAsync(curriculum);

            if (hasSignificantChanges)
            {
                await CreateNewVersionAsync(curriculum.Id, "Updated curriculum details");
            }

            // Update subjects if provided
            if (request.Subjects?.Any() == true)
            {
                await UpdateCurriculumSubjectsAsync(curriculum.Id, request.Subjects);
            }
        }

        public async Task DeleteCurriculumAsync(long id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            curriculum.IsDeleted = true;
            curriculum.DeletedAt = DateTime.UtcNow;
            
            await _curriculumRepository.UpdateAsync(curriculum);
        }

        public async Task<List<GetCurriculumResponse>> GetActiveCurriculaAsync()
        {
            var curricula = await _curriculumRepository.GetActiveCurriculaAsync();
            return _mapper.Map<List<GetCurriculumResponse>>(curricula);
        }

        private async Task AddSubjectsToCurriculumAsync(long curriculumId, List<CurriculumSubjectRequest> subjects)
        {
            foreach (var subjectRequest in subjects)
            {
                var curriculumSubject = new DAL.Entities.CurriculumSubject
                {
                    CurriculumId = curriculumId,
                    SubjectId = subjectRequest.SubjectId,
                    SemesterNumber = subjectRequest.SemesterNumber,
                    IsMandatory = subjectRequest.IsMandatory,
                    CreatedAt = DateTime.UtcNow
                };
                
                await _curriculumSubjectRepository.CreateAsync(curriculumSubject);
            }
        }

        private async Task UpdateCurriculumSubjectsAsync(long curriculumId, List<CurriculumSubjectRequest> subjects)
        {
            // Remove existing subjects
            await _curriculumSubjectRepository.RemoveByCurriculumIdAsync(curriculumId);
            
            // Add new subjects
            await AddSubjectsToCurriculumAsync(curriculumId, subjects);
        }

        private async Task CreateNewVersionAsync(long curriculumId, string changeDescription)
        {
            var latestVersion = await _versionRepository.GetLatestVersionAsync(curriculumId);
            var newVersionNumber = IncrementVersion(latestVersion?.Version ?? "1.0");

            var version = new DAL.Entities.CurriculumVersion
            {
                CurriculumId = curriculumId,
                Version = newVersionNumber,
                EffectiveDate = DateTime.UtcNow,
                ChangeDescription = changeDescription,
                CreatedAt = DateTime.UtcNow
            };

            await _versionRepository.CreateAsync(version);
        }

        private string IncrementVersion(string currentVersion)
        {
            var parts = currentVersion.Split('.');
            if (parts.Length >= 2 && int.TryParse(parts[1], out var minor))
            {
                return $"{parts[0]}.{minor + 1}";
            }
            return "1.1";
        }
    }
}