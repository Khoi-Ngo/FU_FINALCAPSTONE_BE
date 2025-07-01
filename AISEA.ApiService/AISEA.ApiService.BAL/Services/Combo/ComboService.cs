using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Combo;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Combo;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Combo
{
    public class ComboService
    {
        private readonly ComboRepository _comboRepository;
        private readonly ComboSubjectRepository _comboSubjectRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public ComboService(
            ComboRepository comboRepository,
            ComboSubjectRepository comboSubjectRepository,
            SubjectRepository subjectRepository,
            IMapper mapper)
        {
            _comboRepository = comboRepository;
            _comboSubjectRepository = comboSubjectRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateComboAsync(CreateComboRequest request)
        {
            // Check if combo name is unique
            var isNameUnique = await _comboRepository.IsNameUniqueAsync(request.ComboName);
            if (!isNameUnique)
            {
                throw new InvalidUserCreatedException($"Combo with name '{request.ComboName}' already exists.");
            }

            // Validate all subjects exist
            var subjects = await _subjectRepository.GetByIdsAsync(request.SubjectIds);
            if (subjects.Count != request.SubjectIds.Count)
            {
                throw new NotFoundException("One or more subjects not found.");
            }

            var combo = _mapper.Map<DAL.Entities.Combo>(request);
            combo.CreatedAt = DateTime.UtcNow;
            
            await _comboRepository.CreateAsync(combo);

            // Add subjects to combo
            foreach (var subjectId in request.SubjectIds)
            {
                var comboSubject = new DAL.Entities.ComboSubject
                {
                    ComboId = combo.Id,
                    SubjectId = subjectId,
                    CreatedAt = DateTime.UtcNow
                };
                await _comboSubjectRepository.CreateAsync(comboSubject);
            }

            return combo.Id;
        }

        public async Task<PagedResult<GetComboResponse>> GetCombosPagedAsync(PaginationRequest request, string? search = null)
        {
            var (combos, totalCount) = await _comboRepository.GetPagedAsync(request.PageNumber, request.PageSize, search);
            
            return new PagedResult<GetComboResponse>
            {
                Items = _mapper.Map<List<GetComboResponse>>(combos),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetComboDetailResponse> GetComboDetailAsync(long id)
        {
            var combo = await _comboRepository.GetDetailByIdAsync(id);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            return _mapper.Map<GetComboDetailResponse>(combo);
        }

        public async Task UpdateComboAsync(long id, UpdateComboRequest request)
        {
            var combo = await _comboRepository.GetByIdAsync(id);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            // Check if combo name is unique (excluding current combo)
            if (combo.ComboName != request.ComboName)
            {
                var isNameUnique = await _comboRepository.IsNameUniqueAsync(request.ComboName, id);
                if (!isNameUnique)
                {
                    throw new InvalidUserCreatedException($"Combo with name '{request.ComboName}' already exists.");
                }
            }

            _mapper.Map(request, combo);
            combo.UpdatedAt = DateTime.UtcNow;
            
            await _comboRepository.UpdateAsync(combo);
        }

        public async Task DeleteComboAsync(long id)
        {
            var combo = await _comboRepository.GetByIdAsync(id);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            // Remove all subjects from combo first
            await _comboSubjectRepository.RemoveAllSubjectsFromComboAsync(id);

            combo.IsDeleted = true;
            combo.DeletedAt = DateTime.UtcNow;
            
            await _comboRepository.UpdateAsync(combo);
        }

        public async Task AddSubjectToComboAsync(long comboId, long subjectId)
        {
            var combo = await _comboRepository.GetByIdAsync(comboId);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            // Check if subject is already in combo
            var exists = await _comboSubjectRepository.ExistsAsync(comboId, subjectId);
            if (exists)
            {
                throw new InvalidUserCreatedException("Subject is already in this combo.");
            }

            var comboSubject = new DAL.Entities.ComboSubject
            {
                ComboId = comboId,
                SubjectId = subjectId,
                CreatedAt = DateTime.UtcNow
            };

            await _comboSubjectRepository.CreateAsync(comboSubject);
        }

        public async Task<List<ComboSubjectResponse>> GetComboSubjectsAsync(long comboId)
        {
            var combo = await _comboRepository.GetByIdAsync(comboId);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            var comboSubjects = await _comboSubjectRepository.GetByComboIdAsync(comboId);
            return _mapper.Map<List<ComboSubjectResponse>>(comboSubjects);
        }

        public async Task RemoveSubjectFromComboAsync(long comboId, long subjectId)
        {
            var combo = await _comboRepository.GetByIdAsync(comboId);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            var exists = await _comboSubjectRepository.ExistsAsync(comboId, subjectId);
            if (!exists)
            {
                throw new NotFoundException("Subject not found in this combo.");
            }

            await _comboSubjectRepository.RemoveSubjectFromComboAsync(comboId, subjectId);
        }
    }
}