using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Combo;
using AISEA.ApiService.SHARED.DTOs.Responses.Combo;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Combo
{
    public class ComboService
    {
        private readonly ComboRepository _comboRepository;
        private readonly ComboPrerequisiteRepository _prerequisiteRepository;
        private readonly StudentComboEnrollmentRepository _enrollmentRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly UserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;

        public ComboService(
            ComboRepository comboRepository,
            ComboPrerequisiteRepository prerequisiteRepository,
            StudentComboEnrollmentRepository enrollmentRepository,
            SubjectRepository subjectRepository,
            UserRepository userRepository,
            IJWTService jwtService,
            IMapper mapper)
        {
            _comboRepository = comboRepository;
            _prerequisiteRepository = prerequisiteRepository;
            _enrollmentRepository = enrollmentRepository;
            _subjectRepository = subjectRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<long> CreateComboAsync(CreateComboRequest request)
        {
            // Validate subjects exist
            var subjects = await _subjectRepository.GetByIdsAsync(request.SubjectIds);
            if (subjects.Count != request.SubjectIds.Count)
            {
                throw new NotFoundException("One or more subjects not found.");
            }

            var combo = _mapper.Map<DAL.Entities.Combo>(request);
            combo.CreatedAt = DateTime.UtcNow;
            
            await _comboRepository.CreateAsync(combo);

            // Add subjects to combo
            await AddSubjectsToComboAsync(combo.Id, request.SubjectIds);

            // Add prerequisites if provided
            if (request.Prerequisites?.Any() == true)
            {
                await AddPrerequisitesToComboAsync(combo.Id, request.Prerequisites);
            }

            return combo.Id;
        }

        public async Task<PagedResult<GetComboResponse>> GetCombosPagedAsync(ComboSearchRequest request)
        {
            var (combos, totalCount) = await _comboRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.Search,
                request.ProgramId,
                request.SemesterNumber,
                request.DifficultyLevel,
                request.IsAvailable,
                request.SortBy,
                request.SortOrder);
            
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

            _mapper.Map(request, combo);
            combo.UpdatedAt = DateTime.UtcNow;
            
            await _comboRepository.UpdateAsync(combo);

            // Update subjects
            await UpdateComboSubjectsAsync(combo.Id, request.SubjectIds);

            // Update prerequisites
            if (request.Prerequisites?.Any() == true)
            {
                await UpdateComboPrerequisitesAsync(combo.Id, request.Prerequisites);
            }
        }

        public async Task DeleteComboAsync(long id)
        {
            var combo = await _comboRepository.GetByIdAsync(id);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            // Check if there are active enrollments
            var hasActiveEnrollments = await _enrollmentRepository.HasActiveEnrollmentsAsync(id);
            if (hasActiveEnrollments)
            {
                throw new InvalidUserCreatedException("Cannot delete combo with active student enrollments.");
            }

            combo.IsDeleted = true;
            combo.DeletedAt = DateTime.UtcNow;
            
            await _comboRepository.UpdateAsync(combo);
        }

        public async Task<ComboAvailabilityResponse> CheckComboAvailabilityAsync(long comboId, string accessToken)
        {
            var combo = await _comboRepository.GetDetailByIdAsync(comboId);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            var username = _jwtService.GetUsernameFromToken(accessToken);
            var studentProfileId = await _userRepository.GetStudentProfileIdByUsernameAsync(username);

            var availability = new ComboAvailabilityResponse
            {
                ComboId = comboId,
                ComboName = combo.ComboName,
                IsAvailable = true,
                AvailableSlots = combo.MaxStudents - combo.CurrentEnrollment,
                UnavailableReasons = new List<string>(),
                MissingPrerequisites = new List<string>()
            };

            // Check enrollment capacity
            if (combo.CurrentEnrollment >= combo.MaxStudents)
            {
                availability.IsAvailable = false;
                availability.UnavailableReasons.Add("Combo is at full capacity");
            }

            // Check prerequisites
            var missingPrerequisites = await CheckStudentPrerequisitesAsync(studentProfileId, comboId);
            if (missingPrerequisites.Any())
            {
                availability.IsAvailable = false;
                availability.MissingPrerequisites = missingPrerequisites;
                availability.UnavailableReasons.Add("Missing required prerequisites");
            }

            // Check if already enrolled
            var isAlreadyEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(studentProfileId, comboId);
            if (isAlreadyEnrolled)
            {
                availability.IsAvailable = false;
                availability.UnavailableReasons.Add("Student is already enrolled in this combo");
            }

            return availability;
        }

        public async Task EnrollStudentAsync(StudentEnrollmentRequest request)
        {
            var availability = await CheckComboAvailabilityAsync(request.ComboId, ""); // Need to pass actual token
            if (!availability.IsAvailable)
            {
                throw new InvalidUserCreatedException($"Cannot enroll student: {string.Join(", ", availability.UnavailableReasons)}");
            }

            var enrollment = new DAL.Entities.StudentComboEnrollment
            {
                StudentId = request.StudentId,
                ComboId = request.ComboId,
                EnrolledAt = DateTime.UtcNow,
                Status = "Active",
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _enrollmentRepository.CreateAsync(enrollment);
        }

        public async Task BulkEnrollStudentsAsync(BulkEnrollmentRequest request)
        {
            foreach (var studentId in request.StudentIds)
            {
                var enrollmentRequest = new StudentEnrollmentRequest
                {
                    ComboId = request.ComboId,
                    StudentId = studentId,
                    Notes = request.Notes
                };

                try
                {
                    await EnrollStudentAsync(enrollmentRequest);
                }
                catch (Exception)
                {
                    // Log error but continue with other students
                    continue;
                }
            }
        }

        public async Task UnenrollStudentAsync(long comboId, long studentId)
        {
            var enrollment = await _enrollmentRepository.GetByComboAndStudentAsync(comboId, studentId);
            if (enrollment == null)
            {
                throw new NotFoundException("Enrollment not found.");
            }

            enrollment.Status = "Withdrawn";
            enrollment.UpdatedAt = DateTime.UtcNow;
            
            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        private async Task AddSubjectsToComboAsync(long comboId, List<long> subjectIds)
        {
            foreach (var subjectId in subjectIds)
            {
                var comboSubject = new DAL.Entities.ComboSubject
                {
                    ComboId = comboId,
                    SubjectId = subjectId,
                    CreatedAt = DateTime.UtcNow
                };
                
                await _comboRepository.AddSubjectAsync(comboSubject);
            }
        }

        private async Task AddPrerequisitesToComboAsync(long comboId, List<ComboPrerequisiteRequest> prerequisites)
        {
            foreach (var prereq in prerequisites)
            {
                var comboPrerequisite = new DAL.Entities.ComboPrerequisite
                {
                    ComboId = comboId,
                    SubjectId = prereq.SubjectId,
                    IsRequired = prereq.IsRequired,
                    CreatedAt = DateTime.UtcNow
                };
                
                await _prerequisiteRepository.CreateAsync(comboPrerequisite);
            }
        }

        private async Task UpdateComboSubjectsAsync(long comboId, List<long> subjectIds)
        {
            await _comboRepository.RemoveSubjectsByComboIdAsync(comboId);
            await AddSubjectsToComboAsync(comboId, subjectIds);
        }

        private async Task UpdateComboPrerequisitesAsync(long comboId, List<ComboPrerequisiteRequest> prerequisites)
        {
            await _prerequisiteRepository.RemoveByComboIdAsync(comboId);
            await AddPrerequisitesToComboAsync(comboId, prerequisites);
        }

        private async Task<List<string>> CheckStudentPrerequisitesAsync(long studentId, long comboId)
        {
            var prerequisites = await _prerequisiteRepository.GetByComboIdAsync(comboId);
            var missingPrerequisites = new List<string>();

            foreach (var prereq in prerequisites.Where(p => p.IsRequired))
            {
                var hasCompleted = await _enrollmentRepository.HasStudentCompletedSubjectAsync(studentId, prereq.SubjectId);
                if (!hasCompleted)
                {
                    missingPrerequisites.Add(prereq.Subject.SubjectName);
                }
            }

            return missingPrerequisites;
        }
    }
}