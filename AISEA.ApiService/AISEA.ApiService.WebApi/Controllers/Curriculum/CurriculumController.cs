using AISEA.ApiService.BAL.Services.Curriculum;
using AISEA.ApiService.SHARED.DTOs.Requests.Curriculum;
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
        /// Creates a new curriculum with subjects and version tracking
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateCurriculum([FromBody] CreateCurriculumRequest request)
        {
            var curriculumId = await _curriculumService.CreateCurriculumAsync(request);
            return Ok(new { Message = "Curriculum created successfully.", CurriculumId = curriculumId });
        }

        /// <summary>
        /// Gets paginated list of curricula with search and filtering
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCurricula([FromQuery] CurriculumSearchRequest request)
        {
            var result = await _curriculumService.GetCurriculaPagedAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Gets curriculum detail including subjects and version history
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurriculumById(long id)
        {
            var result = await _curriculumService.GetCurriculumDetailAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all active curricula (simplified list)
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCurricula()
        {
            var result = await _curriculumService.GetActiveCurriculaAsync();
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing curriculum and creates new version if needed
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> UpdateCurriculum(long id, [FromBody] UpdateCurriculumRequest request)
        {
            await _curriculumService.UpdateCurriculumAsync(id, request);
            return Ok(new { Message = "Curriculum updated successfully." });
        }

        /// <summary>
        /// Soft deletes a curriculum (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize(1)] // Admin only
        public async Task<IActionResult> DeleteCurriculum(long id)
        {
            await _curriculumService.DeleteCurriculumAsync(id);
            return Ok(new { Message = "Curriculum deleted successfully." });
        }
    }
}