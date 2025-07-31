using AISEA.ApiService.BAL.Services.Curriculum;
using AISEA.ApiService.SHARED.DTOs.Requests.Curriculum;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Curriculum
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurriculumController : BaseController
    {
        private readonly CurriculumService _curriculumService;

        public CurriculumController(EndpointSettings endpointSettings, CurriculumService curriculumService) : base(endpointSettings)
        {
            _curriculumService = curriculumService;
        }

        /// <summary>
        /// Creates a new curriculum (Academic Staff only)
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateCurriculum([FromBody] CreateCurriculumRequest request)
        {
            var accessToken = GetAccessTokenFromHeader();
            var curriculumId = await _curriculumService.CreateCurriculumAsync(request, accessToken);
            return Ok(new { Message = "Curriculum created successfully.", CurriculumId = curriculumId });
        }

        /// <summary>
        /// Creates multiple curicula in bulk (Academic Staff only)
        /// </summary>
        [HttpPost("bulk")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateCurricula([FromBody] List<CreateCurriculumRequest> requests)
        {
            var accessToken = GetAccessTokenFromHeader();
            await _curriculumService.CreateCurriculaAsync(requests, accessToken);
            return Ok(new { Message = "Curricula created successfully." });
        }

        /// <summary>
        /// Gets paginated list of curricula with optional search and program filter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCurricula([FromQuery] PaginationRequest request, [FromQuery] string? search = null, [FromQuery] long? programId = null)
        {
            var result = await _curriculumService.GetCurriculaPagedAsync(request, search, programId);
            return Ok(result);
        }

        /// <summary>
        /// Gets curriculum detail by ID including all subjects
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurriculumById(long id)
        {
            var result = await _curriculumService.GetCurriculumDetailAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing curriculum (Academic Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> UpdateCurriculum(long id, [FromBody] UpdateCurriculumRequest request)
        {
            await _curriculumService.UpdateCurriculumAsync(id, request);
            return Ok(new { Message = "Curriculum updated successfully." });
        }

        /// <summary>
        /// Deletes a curriculum (Admin only) - only if no subjects are assigned
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize(1)] // Admin only
        public async Task<IActionResult> DeleteCurriculum(long id)
        {
            await _curriculumService.DeleteCurriculumAsync(id);
            return Ok(new { Message = "Curriculum deleted successfully." });
        }

        /// <summary>
        /// Gets all subjects within a curriculum
        /// </summary>
        [HttpGet("{id}/subjects")]
        public async Task<IActionResult> GetCurriculumSubjects(long id)
        {
            var result = await _curriculumService.GetCurriculumSubjectsAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Adds a subject to a curriculum (Academic Staff only)
        /// </summary>
        [HttpPost("{id}/subjects")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> AddSubjectToCurriculum(long id, [FromBody] AddSubjectToCurriculumRequest request)
        {
            await _curriculumService.AddSubjectToCurriculumAsync(id, request);
            return Ok(new { Message = "Subject added to curriculum successfully." });
        }

        /// <summary>
        /// Removes a subject version from a curriculum (Academic Staff only)
        /// </summary>
        [HttpDelete("{id}/subjects/{subjectVersionId}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> RemoveSubjectFromCurriculum(long id, long subjectVersionId)
        {
            await _curriculumService.RemoveSubjectFromCurriculumAsync(id, subjectVersionId);
            return Ok(new { Message = "Subject version removed from curriculum successfully." });
        }
    }
}