using AISEA.ApiService.BAL.Services.SubjectVersion;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectVersion;
using AISEA.ApiService.SHARED.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.SubjectVersion
{
    [ApiController]
    [Route("api/subject-version/prerequisites")]
    [Authorize]
    public class SubjectVersionPrerequisiteController : ControllerBase
    {
        private readonly SubjectVersionPrerequisiteService _prerequisiteService;

        public SubjectVersionPrerequisiteController(SubjectVersionPrerequisiteService prerequisiteService)
        {
            _prerequisiteService = prerequisiteService;
        }

        /// <summary>
        /// Add a prerequisite to a subject version
        /// </summary>
        [HttpPost("{subjectVersionId:long}")]
        public async Task<IActionResult> AddPrerequisite(
            long subjectVersionId, 
            [FromBody] AddSubjectVersionPrerequisiteRequest request)
        {
            try
            {
                await _prerequisiteService.AddPrerequisiteAsync(subjectVersionId, request.PrerequisiteSubjectVersionId);
                return Ok(new { message = "Prerequisite added successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidUserCreatedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all prerequisites for a subject version
        /// </summary>
        [HttpGet("{subjectVersionId:long}")]
        public async Task<ActionResult<List<GetSubjectVersionResponse>>> GetPrerequisites(long subjectVersionId)
        {
            try
            {
                var prerequisites = await _prerequisiteService.GetPrerequisitesAsync(subjectVersionId);
                return Ok(prerequisites);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all subject versions that depend on a given subject version (reverse lookup)
        /// </summary>
        [HttpGet("{subjectVersionId:long}/dependents")]
        public async Task<ActionResult<List<GetSubjectVersionResponse>>> GetDependentSubjectVersions(long subjectVersionId)
        {
            try
            {
                var dependents = await _prerequisiteService.GetDependentSubjectVersionsAsync(subjectVersionId);
                return Ok(dependents);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove a prerequisite from a subject version
        /// </summary>
        [HttpDelete("{subjectVersionId:long}/{prerequisiteSubjectVersionId:long}")]
        public async Task<IActionResult> RemovePrerequisite(long subjectVersionId, long prerequisiteSubjectVersionId)
        {
            try
            {
                await _prerequisiteService.RemovePrerequisiteAsync(subjectVersionId, prerequisiteSubjectVersionId);
                return Ok(new { message = "Prerequisite removed successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all prerequisites for all versions of a subject, grouped by version
        /// </summary>
        [HttpGet("by-subject/{subjectId:long}")]
        public async Task<ActionResult<Dictionary<long, List<GetSubjectVersionResponse>>>> GetPrerequisitesBySubjectId(long subjectId)
        {
            var prerequisites = await _prerequisiteService.GetPrerequisitesBySubjectIdGroupedAsync(subjectId);
            return Ok(prerequisites);
        }

        /// <summary>
        /// Copy prerequisites from one subject version to another
        /// </summary>
        [HttpPost("copy")]
        public async Task<IActionResult> CopyPrerequisites([FromBody] CopyPrerequisitesRequest request)
        {
            try
            {
                await _prerequisiteService.CopyPrerequisitesAsync(request.FromSubjectVersionId, request.ToSubjectVersionId);
                return Ok(new { message = "Prerequisites copied successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidUserCreatedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all prerequisites for a subject based on its subject code
        /// Returns unique prerequisites across all active versions of the subject
        /// </summary>
        [HttpGet("by-subject-code/{subjectCode}")]
        public async Task<ActionResult<List<GetSubjectVersionResponse>>> GetPrerequisitesBySubjectCode(string subjectCode)
        {
            try
            {
                var prerequisites = await _prerequisiteService.GetPrerequisitesBySubjectCodeAsync(subjectCode);
                return Ok(prerequisites);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
