using AISEA.ApiService.BAL.Services.Subject;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Subject;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Subject
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : BaseController
    {
        private readonly SubjectService _subjectService;

        public SubjectController(EndpointSettings endpointSettings, SubjectService subjectService) : base(endpointSettings)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// Creates a new subject (Academic Staff only)
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
        {
            await _subjectService.CreateSubjectAsync(request);
            return Ok(new { Message = "Subject created successfully." });
        }

        /// <summary>
        /// Creates multiple subjects in bulk (Academic Staff only)
        /// </summary>
        [HttpPost("bulk")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateSubjects([FromBody] List<CreateSubjectRequest> requests)
        {
            await _subjectService.CreateSubjectsAsync(requests);
            return Ok(new { Message = "Subjects created successfully." });
        }

        /// <summary>
        /// Gets paginated list of subjects with optional search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] PaginationRequest request, [FromQuery] string? search = null)
        {
            var result = await _subjectService.GetSubjectsPagedAsync(request, search);
            return Ok(result);
        }

        /// <summary>
        /// Gets a subject by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(long id)
        {
            var result = await _subjectService.GetSubjectByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing subject (Academic Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> UpdateSubject(long id, [FromBody] UpdateSubjectRequest request)
        {
            await _subjectService.UpdateSubjectAsync(id, request);
            return Ok(new { Message = "Subject updated successfully." });
        }

        /// <summary>
        /// Deletes a subject (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize(1)] // Admin only
        public async Task<IActionResult> DeleteSubject(long id)
        {
            await _subjectService.DeleteSubjectAsync(id);
            return Ok(new { Message = "Subject deleted successfully." });
        }

        /// <summary>
        /// Adds a prerequisite to a subject (Academic Staff only)
        /// </summary>
        [HttpPost("{id}/prerequisites/{prerequisiteId}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> AddPrerequisite(long id, long prerequisiteId)
        {
            await _subjectService.AddPrerequisiteAsync(id, prerequisiteId);
            return Ok(new { Message = "Prerequisite added successfully." });
        }

        /// <summary>
        /// Gets prerequisites for a subject
        /// </summary>
        [HttpGet("{id}/prerequisites")]
        public async Task<IActionResult> GetPrerequisites(long id)
        {
            var result = await _subjectService.GetPrerequisitesAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Removes a prerequisite from a subject (Academic Staff only)
        /// </summary>
        [HttpDelete("{id}/prerequisites/{prerequisiteId}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> RemovePrerequisite(long id, long prerequisiteId)
        {
            await _subjectService.RemovePrerequisiteAsync(id, prerequisiteId);
            return Ok(new { Message = "Prerequisite removed successfully." });
        }
    }
}