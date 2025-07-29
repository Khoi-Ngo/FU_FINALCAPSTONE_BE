using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Syllabus;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Syllabus;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Syllabus
{
    public class SyllabusService
    {
        private readonly SyllabusRepository _syllabusRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly SubjectVersionRepository _subjectVersionRepository;
        private readonly SyllabusAssessmentRepository _assessmentRepository;
        private readonly SyllabusLearningMaterialRepository _materialRepository;
        private readonly SyllabusLearningOutcomeRepository _outcomeRepository;
        private readonly SyllabusSessionRepository _sessionRepository;
        private readonly SessionOutcomeMappingRepository _mappingRepository;
        private readonly IMapper _mapper;

        public SyllabusService(
            SyllabusRepository syllabusRepository,
            SubjectRepository subjectRepository,
            SubjectVersionRepository subjectVersionRepository,
            SyllabusAssessmentRepository assessmentRepository,
            SyllabusLearningMaterialRepository materialRepository,
            SyllabusLearningOutcomeRepository outcomeRepository,
            SyllabusSessionRepository sessionRepository,
            SessionOutcomeMappingRepository mappingRepository,
            IMapper mapper)
        {
            _syllabusRepository = syllabusRepository;
            _subjectRepository = subjectRepository;
            _subjectVersionRepository = subjectVersionRepository;
            _assessmentRepository = assessmentRepository;
            _materialRepository = materialRepository;
            _outcomeRepository = outcomeRepository;
            _sessionRepository = sessionRepository;
            _mappingRepository = mappingRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateSyllabusAsync(CreateSyllabusRequest request)
        {
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(request.SubjectVersionId);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            var existingSyllabus = await _syllabusRepository.GetBySubjectVersionIdAsync(request.SubjectVersionId);
            if (existingSyllabus != null)
            {
                throw new InvalidUserCreatedException("Syllabus for this subject version already exists.");
            }

            var syllabus = _mapper.Map<DAL.Entities.Syllabus>(request);
            syllabus.CreatedAt = DateTime.UtcNow;
            
            await _syllabusRepository.CreateAsync(syllabus);
            return syllabus.Id;
        }

        public async Task<PagedResult<GetSyllabusResponse>> GetSyllabusPagedAsync(PaginationRequest request)
        {
            var (syllabi, totalCount) = await _syllabusRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            
            return new PagedResult<GetSyllabusResponse>
            {
                Items = _mapper.Map<List<GetSyllabusResponse>>(syllabi),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetSyllabusDetailResponse> GetSyllabusDetailAsync(long id)
        {
            var syllabus = await _syllabusRepository.GetDetailByIdAsync(id);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            return _mapper.Map<GetSyllabusDetailResponse>(syllabus);
        }

        public async Task<GetSyllabusDetailResponse> GetSyllabusBySubjectIdAsync(long subjectId)
        {
            // First, validate that the subject exists
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            // Get syllabus using deterministic ordering:
            // 1. Default version first
            // 2. Then active versions
            // 3. Then by most recent effective date
            // 4. Finally by creation date
            var syllabus = await _syllabusRepository.GetBySubjectIdAsync(subjectId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException($"No syllabus found for subject '{subject.SubjectName}' (ID: {subjectId}). " +
                    "Please ensure a syllabus exists for the default or active version of this subject.");
            }

            var detailSyllabus = await _syllabusRepository.GetDetailByIdAsync(syllabus.Id);
            return _mapper.Map<GetSyllabusDetailResponse>(detailSyllabus);
        }

        /// <summary>
        /// Gets the syllabus for the default version of a subject explicitly
        /// </summary>
        public async Task<GetSyllabusDetailResponse> GetSyllabusBySubjectIdDefaultVersionAsync(long subjectId)
        {
            // First, validate that the subject exists
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            // Get syllabus for the default version only
            var syllabus = await _syllabusRepository.GetBySubjectIdDefaultVersionAsync(subjectId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException($"No syllabus found for the default version of subject '{subject.SubjectName}' (ID: {subjectId}). " +
                    "Please ensure a default subject version exists and has a syllabus.");
            }

            var detailSyllabus = await _syllabusRepository.GetDetailByIdAsync(syllabus.Id);
            return _mapper.Map<GetSyllabusDetailResponse>(detailSyllabus);
        }

        /// <summary>
        /// Gets the syllabus for a specific subject version
        /// </summary>
        public async Task<GetSyllabusDetailResponse> GetSyllabusBySubjectVersionIdAsync(long subjectVersionId)
        {
            // First, validate that the subject version exists
            var subjectVersion = await _subjectVersionRepository.GetByIdAsync(subjectVersionId);
            if (subjectVersion == null || subjectVersion.IsDeleted)
            {
                throw new NotFoundException("Subject version not found.");
            }

            // Get syllabus for the specific subject version
            var syllabus = await _syllabusRepository.GetBySubjectVersionIdAsync(subjectVersionId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException($"No syllabus found for subject version '{subjectVersion.VersionName}' (ID: {subjectVersionId}) " +
                    $"of subject '{subjectVersion.Subject?.SubjectName}'. Please ensure a syllabus exists for this version.");
            }

            var detailSyllabus = await _syllabusRepository.GetDetailByIdAsync(syllabus.Id);
            return _mapper.Map<GetSyllabusDetailResponse>(detailSyllabus);
        }

        public async Task UpdateSyllabusAsync(long id, UpdateSyllabusRequest request)
        {
            var syllabus = await _syllabusRepository.GetByIdAsync(id);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            _mapper.Map(request, syllabus);
            syllabus.UpdatedAt = DateTime.UtcNow;
            
            await _syllabusRepository.UpdateAsync(syllabus);
        }

        public async Task DeleteSyllabusAsync(long id)
        {
            var syllabus = await _syllabusRepository.GetByIdAsync(id);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            syllabus.IsDeleted = true;
            syllabus.DeletedAt = DateTime.UtcNow;
            
            await _syllabusRepository.UpdateAsync(syllabus);
        }

        // Assessment methods
        public async Task<long> CreateAssessmentAsync(CreateSyllabusAssessmentRequest request)
        {
            var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            var assessment = _mapper.Map<DAL.Entities.SyllabusAssessment>(request);
            assessment.CreatedAt = DateTime.UtcNow;
            
            await _assessmentRepository.CreateAsync(assessment);
            return assessment.Id;
        }

        // Learning Material methods
        public async Task<long> CreateLearningMaterialAsync(CreateSyllabusLearningMaterialRequest request)
        {
            var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            var material = _mapper.Map<DAL.Entities.SyllabusLearningMaterial>(request);
            material.CreatedAt = DateTime.UtcNow;
            
            await _materialRepository.CreateAsync(material);
            return material.Id;
        }

        // Learning Outcome methods
        public async Task<long> CreateLearningOutcomeAsync(CreateSyllabusLearningOutcomeRequest request)
        {
            var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            var existingOutcome = await _outcomeRepository.GetByCodeAsync(request.SyllabusId, request.OutcomeCode);
            if (existingOutcome != null)
            {
                throw new InvalidUserCreatedException($"Learning outcome with code '{request.OutcomeCode}' already exists for this syllabus.");
            }

            var outcome = _mapper.Map<DAL.Entities.SyllabusLearningOutcome>(request);
            outcome.CreatedAt = DateTime.UtcNow;
            
            await _outcomeRepository.CreateAsync(outcome);
            return outcome.Id;
        }

        // Session methods
        public async Task<long> CreateSessionAsync(CreateSyllabusSessionRequest request)
        {
            var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            var session = _mapper.Map<DAL.Entities.SyllabusSession>(request);
            session.CreatedAt = DateTime.UtcNow;
            
            await _sessionRepository.CreateAsync(session);
            return session.Id;
        }

        public async Task MapSessionToOutcomeAsync(long sessionId, long outcomeId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.IsDeleted)
            {
                throw new NotFoundException("Session not found.");
            }

            var outcome = await _outcomeRepository.GetByIdAsync(outcomeId);
            if (outcome == null || outcome.IsDeleted)
            {
                throw new NotFoundException("Learning outcome not found.");
            }

            var exists = await _mappingRepository.ExistsAsync(sessionId, outcomeId);
            if (exists)
            {
                throw new InvalidUserCreatedException("This session-outcome mapping already exists.");
            }

            var mapping = new DAL.Entities.SessionOutcomeMapping
            {
                SessionId = sessionId,
                OutcomeId = outcomeId,
                CreatedAt = DateTime.UtcNow
            };

            await _mappingRepository.CreateAsync(mapping);
        }

        public async Task<bool> CreateSyllabusAssessmentsAsync(List<CreateSyllabusAssessmentRequest> requests)
        {
            foreach (var request in requests)
            {
                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
                if (syllabus == null || syllabus.IsDeleted)
                {
                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
                }

                var assessment = _mapper.Map<DAL.Entities.SyllabusAssessment>(request);
                assessment.CreatedAt = DateTime.UtcNow;

                await _assessmentRepository.CreateAsync(assessment);
            }
            return true;
        }

        public async Task<bool> CreateSyllabusLearningMaterialsAsync(List<CreateSyllabusLearningMaterialRequest> requests)
        {
            foreach (var request in requests)
            {
                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
                if (syllabus == null || syllabus.IsDeleted)
                {
                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
                }

                var material = _mapper.Map<DAL.Entities.SyllabusLearningMaterial>(request);
                material.CreatedAt = DateTime.UtcNow;
                
                await _materialRepository.CreateAsync(material);
            }
            return true;
        }
        
        public async Task<bool> CreateSyllabusLearningOutcomesAsync(List<CreateSyllabusLearningOutcomeRequest> requests)
        {
            foreach (var request in requests)
            {
                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
                if (syllabus == null || syllabus.IsDeleted)
                {
                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
                }

                var existingOutcome = await _outcomeRepository.GetByCodeAsync(request.SyllabusId, request.OutcomeCode);
                if (existingOutcome != null)
                {
                    throw new InvalidUserCreatedException($"Learning outcome with code '{request.OutcomeCode}' already exists for this syllabus.");
                }

                var outcome = _mapper.Map<DAL.Entities.SyllabusLearningOutcome>(request);
                outcome.CreatedAt = DateTime.UtcNow;
                
                await _outcomeRepository.CreateAsync(outcome);
            }
            return true;
        }
        
        public async Task<bool> CreateSyllabusSessionsAsync(List<CreateSyllabusSessionRequest> requests)
        {
            foreach (var request in requests)
            {
                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
                if (syllabus == null || syllabus.IsDeleted)
                {
                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
                }

                var session = _mapper.Map<DAL.Entities.SyllabusSession>(request);
                session.CreatedAt = DateTime.UtcNow;
                
                await _sessionRepository.CreateAsync(session);
            }
            return true;
        }
    }
}