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
        private readonly CurriculumSubjectRepository _curriculumSubjectRepository;
        private readonly ProgramRepository _programRepository;
        private readonly SubjectVersionRepository _subjectVersionRepository;
        private readonly IMapper _mapper;

        public CurriculumService(
            CurriculumRepository curriculumRepository,
            CurriculumSubjectRepository curriculumSubjectRepository,
            ProgramRepository programRepository,
            SubjectVersionRepository subjectVersionRepository,
            IMapper mapper)
        {
            _curriculumRepository = curriculumRepository;
            _curriculumSubjectRepository = curriculumSubjectRepository;
            _programRepository = programRepository;
            _subjectVersionRepository = subjectVersionRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateCurriculumAsync(CreateCurriculumRequest request)
        {
            // Validate program exists
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null || program.IsDeleted)
            {
                throw new NotFoundException("Program not found.");
            }

            // Check if curriculum code is unique
            var isCodeUnique = await _curriculumRepository.IsCodeUniqueAsync(request.CurriculumCode);
            if (!isCodeUnique)
            {
                throw new InvalidUserCreatedException($"Curriculum with code '{request.CurriculumCode}' already exists.");
            }

            var curriculum = _mapper.Map<DAL.Entities.Curriculum>(request);
            curriculum.CreatedAt = DateTime.UtcNow;
            
            await _curriculumRepository.CreateAsync(curriculum);
            return curriculum.Id;
        }

        public async Task<bool> CreateCurriculaAsync(List<CreateCurriculumRequest> requests)
        {
            foreach(var request in requests)
            {
                //Validate program exists
                var program = await _programRepository.GetByIdAsync(request.ProgramId);
                if (program == null || program.IsDeleted)
                {
                    throw new NotFoundException($"Program with ID {request.ProgramId} not found.");
                }
                //Check if curriculum code is unique
                var isUniqueCode = await _curriculumRepository.IsCodeUniqueAsync(request.CurriculumCode);
                if (!isUniqueCode)
                {
                       throw new InvalidUserCreatedException($"Curriculum with code '{request.CurriculumCode}' already exists.");
                }
                var curriculum = _mapper.Map<DAL.Entities.Curriculum>(request);
                curriculum.CreatedAt = DateTime.UtcNow;

                await _curriculumRepository.CreateAsync(curriculum);
            }
            return true;
        }

        public async Task<PagedResult<GetCurriculumResponse>> GetCurriculaPagedAsync(PaginationRequest request, string? search = null, long? programId = null)
        {
            var (curricula, totalCount) = await _curriculumRepository.GetPagedAsync(request.PageNumber, request.PageSize, search, programId);
            
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

            // Validate program exists
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null || program.IsDeleted)
            {
                throw new NotFoundException("Program not found.");
            }

            // Check if curriculum code is unique (excluding current curriculum)
            if (curriculum.CurriculumCode != request.CurriculumCode)
            {
                var isCodeUnique = await _curriculumRepository.IsCodeUniqueAsync(request.CurriculumCode, id);
                if (!isCodeUnique)
                {
                    throw new InvalidUserCreatedException($"Curriculum with code '{request.CurriculumCode}' already exists.");
                }
            }

            _mapper.Map(request, curriculum);
            curriculum.UpdatedAt = DateTime.UtcNow;
            
            await _curriculumRepository.UpdateAsync(curriculum);
        }

        public async Task DeleteCurriculumAsync(long id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            // Check if curriculum has subjects
            var hasSubjects = await _curriculumRepository.HasSubjectsAsync(id);
            if (hasSubjects)
            {
                throw new InvalidUserCreatedException("Cannot delete curriculum that contains subjects. Please remove all subjects first.");
            }

            curriculum.IsDeleted = true;
            curriculum.DeletedAt = DateTime.UtcNow;
            
            await _curriculumRepository.UpdateAsync(curriculum);
        }

        public async Task AddSubjectToCurriculumAsync(long curriculumId, AddSubjectToCurriculumRequest request)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            var subjectVersion = await _subjectVersionRepository.GetByIdWithSubjectAsync(request.SubjectVersionId);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            // Check if a different version of the same subject already exists
            var hasSubjectWithSameCode = await _curriculumSubjectRepository.HasSubjectWithSubjectCodeAsync(curriculumId, subjectVersion.Subject.SubjectCode);
            if (hasSubjectWithSameCode)
            {
                throw new InvalidUserCreatedException($"A version of subject with code '{subjectVersion.Subject.SubjectCode}' already exists in this curriculum.");
            }

            var curriculumSubject = _mapper.Map<DAL.Entities.CurriculumSubject>(request);
            curriculumSubject.CurriculumId = curriculumId;
            curriculumSubject.CreatedAt = DateTime.UtcNow;

            await _curriculumSubjectRepository.CreateAsync(curriculumSubject);
        }

        public async Task<List<CurriculumSubjectResponse>> GetCurriculumSubjectsAsync(long curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            var curriculumSubjects = await _curriculumSubjectRepository.GetByCurriculumIdAsync(curriculumId);
            return _mapper.Map<List<CurriculumSubjectResponse>>(curriculumSubjects);
        }

        public async Task RemoveSubjectFromCurriculumAsync(long curriculumId, long subjectVersionId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            var exists = await _curriculumSubjectRepository.ExistsAsync(curriculumId, subjectVersionId);
            if (!exists)
            {
                throw new NotFoundException("Subject version not found in this curriculum.");
            }

            await _curriculumSubjectRepository.RemoveSubjectFromCurriculumAsync(curriculumId, subjectVersionId);
        }
    }
}