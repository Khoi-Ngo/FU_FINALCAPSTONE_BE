using AISEA.ApiService.BAL.Services.SubjectVersion;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.SubjectVersion
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectVersionController : BaseController
    {
        private readonly SubjectVersionService _subjectVersionService;

        public SubjectVersionController(EndpointSettings endpointSettings, SubjectVersionService subjectVersionService) 
            : base(endpointSettings)
        {
            _subjectVersionService = subjectVersionService;
        }

        /// <summary>
        /// Creates a new subject version (Academic Staff only)
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateSubjectVersion([FromBody] CreateSubjectVersionRequest request)
        {
            await _subjectVersionService.CreateSubjectVersionAsync(request);
            return Ok(new { Message = "Subject version created successfully." });
        }

        /// <summary>
        /// Gets paginated list of subject versions with optional filters
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSubjectVersions(
            [FromQuery] PaginationRequest request,
            [FromQuery] long? subjectId = null,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var result = await _subjectVersionService.GetSubjectVersionsPagedAsync(request, subjectId, search, isActive);
            return Ok(result);
        }

        /// <summary>
        /// Gets a subject version by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectVersionById(long id)
        {
            var result = await _subjectVersionService.GetSubjectVersionByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all versions for a specific subject
        /// </summary>
        [HttpGet("subject/{subjectId}")]
        public async Task<IActionResult> GetVersionsBySubjectId(long subjectId, [FromQuery] bool activeOnly = false)
        {
            var result = await _subjectVersionService.GetSubjectVersionsBySubjectIdAsync(subjectId, activeOnly);
            return Ok(result);
        }

        /// <summary>
        /// Gets the default version for a specific subject
        /// </summary>
        [HttpGet("subject/{subjectId}/default")]
        public async Task<IActionResult> GetDefaultVersion(long subjectId)
        {
            var result = await _subjectVersionService.GetDefaultVersionAsync(subjectId);
            if (result == null)
            {
                return NotFound(new { Message = "No default version found for this subject." });
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets all currently active versions (effective as of specified date or now)
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveVersions([FromQuery] DateTime? asOfDate = null)
        {
            var result = await _subjectVersionService.GetActiveVersionsAsync(asOfDate);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing subject version (Academic Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> UpdateSubjectVersion(long id, [FromBody] UpdateSubjectVersionRequest request)
        {
            await _subjectVersionService.UpdateSubjectVersionAsync(id, request);
            return Ok(new { Message = "Subject version updated successfully." });
        }

        /// <summary>
        /// Deletes a subject version (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize(1)] // Admin only
        public async Task<IActionResult> DeleteSubjectVersion(long id)
        {
            await _subjectVersionService.DeleteSubjectVersionAsync(id);
            return Ok(new { Message = "Subject version deleted successfully." });
        }

        /// <summary>
        /// Sets a version as the default for its subject (Academic Staff only)
        /// </summary>
        [HttpPost("{id}/set-default")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> SetDefaultVersion(long id)
        {
            await _subjectVersionService.SetDefaultVersionAsync(id);
            return Ok(new { Message = "Version set as default successfully." });
        }

        /// <summary>
        /// Toggles the active status of a version (Academic Staff only)
        /// </summary>
        [HttpPost("{id}/toggle-active")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> ToggleActiveStatus(long id)
        {
            await _subjectVersionService.ToggleActiveStatusAsync(id);
            return Ok(new { Message = "Version status toggled successfully." });
        }
    }
}
