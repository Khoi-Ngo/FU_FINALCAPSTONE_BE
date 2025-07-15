using AISEA.ApiService.BAL.Services.Syllabus;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Syllabus;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Syllabus
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyllabusController : BaseController
    {
        private readonly SyllabusService _syllabusService;

        public SyllabusController(EndpointSettings endpointSettings, SyllabusService syllabusService) : base(endpointSettings)
        {
            _syllabusService = syllabusService;
        }

        /// <summary>
        /// Creates a new syllabus (Academic Staff only)
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateSyllabus([FromBody] CreateSyllabusRequest request)
        {
            var syllabusId = await _syllabusService.CreateSyllabusAsync(request);
            return Ok(new { Message = "Syllabus created successfully.", SyllabusId = syllabusId });
        }

        /// <summary>
        /// Gets paginated list of syllabi
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSyllabi([FromQuery] PaginationRequest request)
        {
            var result = await _syllabusService.GetSyllabusPagedAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Gets syllabus detail by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSyllabusById(long id)
        {
            var result = await _syllabusService.GetSyllabusDetailAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets syllabus by subject ID
        /// </summary>
        [HttpGet("by-subject/{subjectId}")]
        public async Task<IActionResult> GetSyllabusBySubjectId(long subjectId)
        {
            var result = await _syllabusService.GetSyllabusBySubjectIdAsync(subjectId);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing syllabus (Academic Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> UpdateSyllabus(long id, [FromBody] UpdateSyllabusRequest request)
        {
            await _syllabusService.UpdateSyllabusAsync(id, request);
            return Ok(new { Message = "Syllabus updated successfully." });
        }

        /// <summary>
        /// Deletes a syllabus (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize(1)] // Admin only
        public async Task<IActionResult> DeleteSyllabus(long id)
        {
            await _syllabusService.DeleteSyllabusAsync(id);
            return Ok(new { Message = "Syllabus deleted successfully." });
        }

        /// <summary>
        /// Creates an assessment for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("assessments")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)] // Admin, Academic Staff
        public async Task<IActionResult> CreateAssessment([FromBody] CreateSyllabusAssessmentRequest request)
        {
            var assessmentId = await _syllabusService.CreateAssessmentAsync(request);
            return Ok(new { Message = "Assessment created successfully.", AssessmentId = assessmentId });
        }

        /// <summary>
        /// Creates a learning material for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("materials")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)] // Admin, Academic Staff
        public async Task<IActionResult> CreateLearningMaterial([FromBody] CreateSyllabusLearningMaterialRequest request)
        {
            var materialId = await _syllabusService.CreateLearningMaterialAsync(request);
            return Ok(new { Message = "Learning material created successfully.", MaterialId = materialId });
        }

        /// <summary>
        /// Creates a learning outcome for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("outcomes")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)] // Admin, Academic Staff
        public async Task<IActionResult> CreateLearningOutcome([FromBody] CreateSyllabusLearningOutcomeRequest request)
        {
            var outcomeId = await _syllabusService.CreateLearningOutcomeAsync(request);
            return Ok(new { Message = "Learning outcome created successfully.", OutcomeId = outcomeId });
        }

        /// <summary>
        /// Creates a session for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("sessions")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF | (int)EUserRole.ADMIN)] // Admin, Academic Staff
        public async Task<IActionResult> CreateSession([FromBody] CreateSyllabusSessionRequest request)
        {
            var sessionId = await _syllabusService.CreateSessionAsync(request);
            return Ok(new { Message = "Session created successfully.", SessionId = sessionId });
        }

        /// <summary>
        /// Maps a session to a learning outcome (Academic Staff only)
        /// </summary>
        [HttpPost("sessions/{sessionId}/outcomes/{outcomeId}")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)] // Admin, Academic Staff
        public async Task<IActionResult> MapSessionToOutcome(long sessionId, long outcomeId)
        {
            await _syllabusService.MapSessionToOutcomeAsync(sessionId, outcomeId);
            return Ok(new { Message = "Session mapped to outcome successfully." });
        }

        /// <summary>
        /// Creates multiple assessments for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("assessments/bulk")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)] // Admin, Academic Staff
        public async Task<IActionResult> CreateAssessments([FromBody] List<CreateSyllabusAssessmentRequest> requests)
        {
            var assessmentId = await _syllabusService.CreateSyllabusAssessmentsAsync(requests);
            return Ok(new { Message = "Assessment created successfully.", AssessmentId = assessmentId });
        }

        /// <summary>
        /// Creates multiple learning materials for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("materials/bulk")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)] // Admin, Academic Staff
        public async Task<IActionResult> CreateLearningMaterials([FromBody] List<CreateSyllabusLearningMaterialRequest> requests)
        {
            var materialId = await _syllabusService.CreateSyllabusLearningMaterialsAsync(requests);
            return Ok(new { Message = "Learning material created successfully.", MaterialId = materialId });
        }

        /// <summary>
        /// Creates multiple learning outcomes for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("outcomes/bulk")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)] // Admin, Academic Staff
        public async Task<IActionResult> CreateLearningOutcomes([FromBody] List<CreateSyllabusLearningOutcomeRequest> requests)
        {
            var outcomeId = await _syllabusService.CreateSyllabusLearningOutcomesAsync(requests);
            return Ok(new { Message = "Learning outcome created successfully.", OutcomeId = outcomeId });
        }

        /// <summary>
        /// Creates multiple sessions for a syllabus (Academic Staff only)
        /// </summary>
        [HttpPost("sessions/bulk")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF | (int)EUserRole.ADMIN)] // Admin, Academic Staff
        public async Task<IActionResult> CreateSessions([FromBody] List<CreateSyllabusSessionRequest> requests)
        {
            var sessionId = await _syllabusService.CreateSyllabusSessionsAsync(requests);
            return Ok(new { Message = "Session created successfully.", SessionId = sessionId });
        }
        

    }
}