using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Syllabus;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Syllabus;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
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
        private readonly IJWTService _jwtService;
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
            IJWTService jwtService,
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
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<long> CreateSyllabusAsync(CreateSyllabusRequest request, string accessToken)
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

            var createdBy = _jwtService.GetUsernameFromToken(accessToken);
            var syllabus = _mapper.Map<DAL.Entities.Syllabus>(request);
            syllabus.CreatedBy = createdBy;
            syllabus.CreatedAt = DateTime.UtcNow;
            
            await _syllabusRepository.CreateAsync(syllabus);
            return syllabus.Id;
        }

        public async Task<PagedResult<GetSyllabusResponse>> GetSyllabusPagedAsync(PaginationRequest request, string? subjectCodeSearch = null)
        {
            var (syllabi, totalCount) = await _syllabusRepository.GetPagedAsync(request.PageNumber, request.PageSize, subjectCodeSearch);

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

            // Check if session number already exists for this syllabus
            var sessionNumberExists = await _sessionRepository.ExistsSessionNumberAsync(request.SyllabusId, request.SessionNumber);
            if (sessionNumberExists)
            {
                throw new InvalidUserCreatedException($"Session number {request.SessionNumber} already exists for this syllabus. Session numbers must be unique within each syllabus.");
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

            // Check if an active mapping already exists
            var exists = await _mappingRepository.ExistsAsync(sessionId, outcomeId);
            if (exists)
            {
                throw new InvalidUserCreatedException("This session-outcome mapping already exists.");
            }

            // Check if a soft-deleted mapping exists and reactivate it
            var deletedMapping = await _mappingRepository.GetDeletedMappingAsync(sessionId, outcomeId);
            if (deletedMapping != null)
            {
                // Reactivate the existing mapping
                deletedMapping.IsDeleted = false;
                deletedMapping.DeletedAt = null;
                deletedMapping.UpdatedAt = DateTime.UtcNow;
                
                await _mappingRepository.UpdateAsync(deletedMapping);
                return;
            }

            // Create new mapping if no existing mapping found
            var mapping = new DAL.Entities.SessionOutcomeMapping
            {
                SessionId = sessionId,
                OutcomeId = outcomeId,
                CreatedAt = DateTime.UtcNow
            };

            await _mappingRepository.CreateAsync(mapping);
        }

        public async Task UnmapSessionFromOutcomeAsync(long sessionId, long outcomeId)
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

            var mapping = await _mappingRepository.GetBySessionIdAndOutcomeIdAsync(sessionId, outcomeId);
            if (mapping == null)
            {
                throw new NotFoundException("Session-outcome mapping not found.");
            }

            mapping.IsDeleted = true;
            mapping.DeletedAt = DateTime.UtcNow;
            
            await _mappingRepository.UpdateAsync(mapping);
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
            // Validate all requests first
            var syllabusSessionNumbers = new Dictionary<long, HashSet<int>>();
            
            foreach (var request in requests)
            {
                var syllabus = await _syllabusRepository.GetByIdAsync(request.SyllabusId);
                if (syllabus == null || syllabus.IsDeleted)
                {
                    throw new NotFoundException($"Syllabus with ID {request.SyllabusId} not found.");
                }

                // Track session numbers within this batch
                if (!syllabusSessionNumbers.ContainsKey(request.SyllabusId))
                {
                    syllabusSessionNumbers[request.SyllabusId] = new HashSet<int>();
                }
                
                // Check for duplicates within the batch
                if (syllabusSessionNumbers[request.SyllabusId].Contains(request.SessionNumber))
                {
                    throw new InvalidUserCreatedException($"Duplicate session number {request.SessionNumber} found in the batch for syllabus ID {request.SyllabusId}.");
                }
                
                syllabusSessionNumbers[request.SyllabusId].Add(request.SessionNumber);
                
                // Check if session number already exists in database
                var sessionNumberExists = await _sessionRepository.ExistsSessionNumberAsync(request.SyllabusId, request.SessionNumber);
                if (sessionNumberExists)
                {
                    throw new InvalidUserCreatedException($"Session number {request.SessionNumber} already exists for syllabus ID {request.SyllabusId}. Session numbers must be unique within each syllabus.");
                }
            }
            
            // If all validations pass, create the sessions
            foreach (var request in requests)
            {
                var session = _mapper.Map<DAL.Entities.SyllabusSession>(request);
                session.CreatedAt = DateTime.UtcNow;
                
                await _sessionRepository.CreateAsync(session);
            }
            return true;
        }

        // Assessment UPDATE and DELETE methods
        public async Task UpdateAssessmentAsync(long id, UpdateSyllabusAssessmentRequest request)
        {
            var assessment = await _assessmentRepository.GetByIdAsync(id);
            if (assessment == null || assessment.IsDeleted)
            {
                throw new NotFoundException("Assessment not found.");
            }

            _mapper.Map(request, assessment);
            assessment.UpdatedAt = DateTime.UtcNow;
            
            await _assessmentRepository.UpdateAsync(assessment);
        }

        public async Task DeleteAssessmentAsync(long id)
        {
            var assessment = await _assessmentRepository.GetByIdAsync(id);
            if (assessment == null || assessment.IsDeleted)
            {
                throw new NotFoundException("Assessment not found.");
            }

            assessment.IsDeleted = true;
            assessment.DeletedAt = DateTime.UtcNow;
            
            await _assessmentRepository.UpdateAsync(assessment);
        }

        // Learning Material UPDATE and DELETE methods
        public async Task UpdateLearningMaterialAsync(long id, UpdateSyllabusLearningMaterialRequest request)
        {
            var material = await _materialRepository.GetByIdAsync(id);
            if (material == null || material.IsDeleted)
            {
                throw new NotFoundException("Learning material not found.");
            }

            _mapper.Map(request, material);
            material.UpdatedAt = DateTime.UtcNow;
            
            await _materialRepository.UpdateAsync(material);
        }

        public async Task DeleteLearningMaterialAsync(long id)
        {
            var material = await _materialRepository.GetByIdAsync(id);
            if (material == null || material.IsDeleted)
            {
                throw new NotFoundException("Learning material not found.");
            }

            material.IsDeleted = true;
            material.DeletedAt = DateTime.UtcNow;
            
            await _materialRepository.UpdateAsync(material);
        }

        // Learning Outcome UPDATE and DELETE methods
        public async Task UpdateLearningOutcomeAsync(long id, UpdateSyllabusLearningOutcomeRequest request)
        {
            var outcome = await _outcomeRepository.GetByIdAsync(id);
            if (outcome == null || outcome.IsDeleted)
            {
                throw new NotFoundException("Learning outcome not found.");
            }

            // Check if the new outcome code already exists for the same syllabus (excluding current record)
            var existingOutcome = await _outcomeRepository.GetByCodeAsync(outcome.SyllabusId, request.OutcomeCode);
            if (existingOutcome != null && existingOutcome.Id != id)
            {
                throw new InvalidUserCreatedException($"Learning outcome with code '{request.OutcomeCode}' already exists for this syllabus.");
            }

            _mapper.Map(request, outcome);
            outcome.UpdatedAt = DateTime.UtcNow;
            
            await _outcomeRepository.UpdateAsync(outcome);
        }

        public async Task DeleteLearningOutcomeAsync(long id)
        {
            var outcome = await _outcomeRepository.GetByIdAsync(id);
            if (outcome == null || outcome.IsDeleted)
            {
                throw new NotFoundException("Learning outcome not found.");
            }

            outcome.IsDeleted = true;
            outcome.DeletedAt = DateTime.UtcNow;
            
            await _outcomeRepository.UpdateAsync(outcome);
        }

        // Session UPDATE and DELETE methods
        public async Task UpdateSessionAsync(long id, UpdateSyllabusSessionRequest request)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null || session.IsDeleted)
            {
                throw new NotFoundException("Session not found.");
            }

            // Check if the new session number already exists for this syllabus (excluding current session)
            var sessionNumberExists = await _sessionRepository.ExistsSessionNumberAsync(session.SyllabusId, request.SessionNumber, id);
            if (sessionNumberExists)
            {
                throw new InvalidUserCreatedException($"Session number {request.SessionNumber} already exists for this syllabus. Session numbers must be unique within each syllabus.");
            }

            _mapper.Map(request, session);
            session.UpdatedAt = DateTime.UtcNow;
            
            await _sessionRepository.UpdateAsync(session);
        }

        public async Task DeleteSessionAsync(long id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null || session.IsDeleted)
            {
                throw new NotFoundException("Session not found.");
            }

            session.IsDeleted = true;
            session.DeletedAt = DateTime.UtcNow;
            
            await _sessionRepository.UpdateAsync(session);
        }
    }
}